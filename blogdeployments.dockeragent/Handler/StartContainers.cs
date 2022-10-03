using blogdeployments.dockeragent;
using Docker.DotNet;
using Docker.DotNet.Models;
using MediatR;
using Microsoft.Extensions.Options;

public class StartContainers : IRequest<bool>
{
    public class StartDatabaseHandler : IRequestHandler<StartContainers, bool>
    {
        private readonly ContainerConfig _containerConfig;

        public StartDatabaseHandler(IOptions<ContainerConfig> containerConfig)
        {
            _containerConfig = containerConfig.Value;
        }

        public async Task<bool> Handle(StartContainers request, CancellationToken cancellationToken)
        {
            var started = false;
            var socket = Environment.OSVersion.Platform switch
            {
                PlatformID.Win32NT => "npipe://./pipe/docker_engine",
                PlatformID.MacOSX or PlatformID.Unix => "unix:///var/run/docker.sock",
                _ => throw new ApplicationException($"Platform {Environment.OSVersion.Platform} is not supported.")
            };

            DockerClient client = new DockerClientConfiguration(
                    new Uri(socket))
                .CreateClient();

            foreach (var container in _containerConfig.Containers)
            {
                await client.Images.CreateImageAsync(
                    new ImagesCreateParameters
                    {
                        FromImage = container.Image,
                        Tag = container.Tag,
                    },
                    null
                    ,
                    new Progress<JSONMessage>(), cancellationToken);

                var exposedPorts = new Dictionary<string, EmptyStruct>(container.Ports.Select(p =>
                    new KeyValuePair<string, EmptyStruct>(p.Container.ToString(), new EmptyStruct())
                ));

                var mounts = new List<string>(
                    container.Mounts.Select(m => $"{ReplaceTokens(m.Source)}:{m.Target}")
                );

                var portBindings = new Dictionary<string, IList<Docker.DotNet.Models.PortBinding>>(
                    container.Ports.Select(p => new KeyValuePair<string, IList<Docker.DotNet.Models.PortBinding>>(
                        p.Container.ToString(),
                        new List<Docker.DotNet.Models.PortBinding>(
                            p.HostBindings.Select(hb => new Docker.DotNet.Models.PortBinding
                            {
                                HostPort = hb.HostPort.ToString(),
                                HostIP = hb.HostIp
                            })
                        )))
                );

                var createContainerResponse = await client.Containers.CreateContainerAsync(
                    new CreateContainerParameters()
                    {
                        Image = $"{container.Image}:{container.Tag}",
                        Name = container.Name,
                        // https://github.com/dotnet/Docker.DotNet/issues/134#issuecomment-292053084
                        // Env = new[] { "ACCEPT_EULA=Y", $"SA_PASSWORD={s_saPassword}" },
                        ExposedPorts = exposedPorts,
                        HostConfig = new HostConfig()
                        {
                            Binds = mounts,
                            PortBindings = portBindings,
                        }
                    }, cancellationToken);

                started = await client.Containers.StartContainerAsync(
                    createContainerResponse.ID,
                    new ContainerStartParameters(), cancellationToken);
            }

            return started;
        }

        private static string ReplaceTokens(string tokenString)
        {
            var homePath = Environment.OSVersion.Platform is PlatformID.Unix or PlatformID.MacOSX
                ? Environment.GetEnvironmentVariable("HOME")
                : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
            
            return tokenString.Replace("$HOME", homePath);
        }
    }
}