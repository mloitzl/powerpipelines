using AutoMapper;
using blogdeployments.domain.Events;
using blogdeployments.power.Handler;
using Xunit;

namespace blogdeployments.power.tests;

public class DomainEventsMappingTests
{
    [Fact]
    public void Test_ShutdownInitiated_CheckHostStatusMapping_MapsCorrectly()
    {
        var configuration = new MapperConfiguration(cfg =>
            cfg.CreateMap<ShutdownInitiated, CheckHostStatus>());

        configuration.AssertConfigurationIsValid();

        var mapper = configuration.CreateMapper();
        var @event = new ShutdownInitiated
        {
            Hostname = "asasdas"
        };

        var expected = mapper.Map<CheckHostStatus>(@event);

        Assert.Equal(expected.Hostname, @event.Hostname);
    }
}