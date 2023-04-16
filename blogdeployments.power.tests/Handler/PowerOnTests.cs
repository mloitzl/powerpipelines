using System;
using System.Threading.Tasks;
using blogdeployments.domain;
using blogdeployments.power.Handler;
using blogdeployments.power.Service;
using blogdeployments.repository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace blogdeployments.power.tests.Handler;

public class PowerOnTests
{
    [Fact]
    public async Task Test_ShutdownInitiated_CheckHostStatusMapping_MapsCorrectly()
    {
        var clusterPowerStatusRepositoryMock = new Mock<IClusterPowerStatusRepository>();
        var mockLoggerPowerOnHandler = new Mock<ILogger<PowerOn.PowerOnHandler>>();
        var mockLoggerRaspbeeService = new Mock<ILogger<RaspbeeService>>();
        var mockRaspbeeService = new Mock<IRaspbeeService>();

        var powerStatus = new ClusterPower();

        clusterPowerStatusRepositoryMock.Setup(repository => repository.EnsureHostPowerStatus(
            Guid.Empty.ToString(),
            "Dummy",
             new HostPowerStatus
             {

                 Status = PowerStatus.Unknown,
                 Hostname = "Dummy"
             }))
        .ReturnsAsync(new HostPowerStatus { });

        mockRaspbeeService.Setup( raspbeeService => raspbeeService.PowerOn()).Returns(true);

        var clusterTopologyConfiguration = Options.Create(new ClusterTopologyConfiguration
        {
            Hosts = new string[] { "Dummy" }
        });

        var raspbeeConfiguration = Options.Create(new RaspbeeConfiguration());

        IRaspbeeService raspbeeService = mockRaspbeeService.Object;

        var sut = new PowerOn.PowerOnHandler(clusterPowerStatusRepositoryMock.Object, clusterTopologyConfiguration, mockLoggerPowerOnHandler.Object, raspbeeService);

        var result = await sut.Handle(new PowerOn
        {
            RequestId = Guid.Empty

        }, new System.Threading.CancellationToken());

        Assert.True(result);
    }
}
