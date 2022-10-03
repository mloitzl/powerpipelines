using Docker.DotNet;
using Docker.DotNet.Models;
using MediatR;

public class StartDatabase : IRequest<bool>
{
    public class StartDatabaseHandler : IRequestHandler<StartDatabase, bool>
    {
        public async Task<bool> Handle(StartDatabase request, CancellationToken cancellationToken)
        {
            DockerClient client = new DockerClientConfiguration(
                    new Uri("unix:///var/run/docker.sock"))
                .CreateClient();
            IList<ContainerListResponse> containers = await client.Containers.ListContainersAsync(
                new ContainersListParameters()
                {
                    Limit = 10,
                });

            await client.Images.CreateImageAsync(
                new ImagesCreateParameters
                {
                    FromImage = "couchdb",
                    Tag = "3.2.2",
                },
                null
                ,
                new Progress<JSONMessage>());

            var createContainerResponse = await client.Containers.CreateContainerAsync(new CreateContainerParameters()
            {
                Image = "couchdb",
                Name = "started_from_netcore",
                // https://github.com/dotnet/Docker.DotNet/issues/134#issuecomment-292053084
                // Env = new[] { "ACCEPT_EULA=Y", $"SA_PASSWORD={s_saPassword}" },
                // ExposedPorts = new Dictionary<string, EmptyStruct>() {
                //     { 1433.ToString(), new EmptyStruct()}
                //},

                HostConfig = new HostConfig()
                {
                    Binds = new List<string>()
                    {
                        "/Users/martin/.secrets/couchdb/local.ini:/opt/couchdb/etc/local.ini"
                    },
                    // https://github.com/dotnet/Docker.DotNet/issues/134#issuecomment-292053084
                    // PortBindings = new Dictionary<string, IList<PortBinding>>
                    // {
                    //     {
                    //         1433.ToString(), new List<PortBinding>
                    //         {
                    //             new PortBinding {HostPort = 1433.ToString()}
                    //         }
                    //     }
                    // },
                    //DNS = new[] { "8.8.8.8", "8.8.4.4" }
                }
            });

            var started = await client.Containers.StartContainerAsync(
                createContainerResponse.ID,
                new ContainerStartParameters()
            );

            Console.WriteLine("Issue Start Container Commandline");
            return true;
        }
    }
}