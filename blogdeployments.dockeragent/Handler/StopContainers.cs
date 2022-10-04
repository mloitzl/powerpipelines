using blogdeployments.dockeragent;
using Docker.DotNet;
using Docker.DotNet.Models;
using MediatR;
using Microsoft.Extensions.Options;

public class StopContainers : IRequest<bool>
{
    public class StopDatabaseHandler : IRequestHandler<StopContainers, bool>
    {
        private readonly ContainerConfig _containerConfig;

        public StopDatabaseHandler(IOptions<ContainerConfig> config)
        {
            _containerConfig = config.Value;
        }

        public async Task<bool> Handle(StopContainers request, CancellationToken cancellationToken)
        {
            var stopped = false;

            var socket = Environment.OSVersion.Platform switch
            {
                PlatformID.Win32NT => "npipe://./pipe/docker_engine",
                PlatformID.MacOSX or PlatformID.Unix => "unix:///var/run/docker.sock",
                _ => throw new ApplicationException($"Platform {Environment.OSVersion.Platform} is not supported.")
            };

            var client = new DockerClientConfiguration(
                    new Uri(socket))
                .CreateClient();

            IList<ContainerListResponse> containers = await client.Containers.ListContainersAsync(
                new ContainersListParameters(), cancellationToken);

            foreach (var container in _containerConfig.Containers)
            {
                var toBeStoppedIds = containers
                    .Where(c =>
                        c.Names.Contains($"/{container.Name}"))
                    .Select(c => c.ID);

                foreach (var stopId in toBeStoppedIds)
                {
                    stopped = await client
                        .Containers
                        .StopContainerAsync(stopId,
                            new ContainerStopParameters(),
                            cancellationToken);
                    await client
                        .Containers
                        .RemoveContainerAsync(stopId,
                            new ContainerRemoveParameters(),
                            cancellationToken);
                }
            }

            return stopped;
        }
    }
}