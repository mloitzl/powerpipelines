using System;
using System.Reflection;
using AutoMapper;
using blogdeployments.domain;
using blogdeployments.repository.Model;
using CouchDB.Driver.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;
using Xunit;

namespace blogdeployments.repository.tests;

[Trait("Category", "Integration")]
[assembly: UserSecretsId("blogdeployments.repository.tests")]
public class HostPowerStatusIntegrationTests
{
    private readonly DeploymentsContext _context;
    private readonly IMapper _mapper;
    private readonly string docId = "7b018418-b99e-4e8f-980b-327fba932a4e";

    public HostPowerStatusIntegrationTests()
    {
        var builder = new ConfigurationBuilder()
            .AddUserSecrets(Assembly.GetExecutingAssembly(), true);

        var configuration = builder.Build();

        var mappingConfig = new MapperConfiguration(mc => { mc.AddProfile(new MappingProfile()); });

        _mapper = mappingConfig.CreateMapper();
        var couchOptionsBuilder = new CouchOptionsBuilder<DeploymentsContext>();
        couchOptionsBuilder
            .UseEndpoint("http://localhost:5984")
            .EnsureDatabaseExists()
            .UseBasicAuthentication(configuration["couchdb:user"], configuration["couchdb:password"]);

        _context = new DeploymentsContext(couchOptionsBuilder.Options);
    }

    [Fact]
    public async void UnknownStatus()
    {
        IClusterPowerStatusRepository sut = new ClusterPowerStatusRepository(_context, _mapper);

        var powerStatus = await sut.EnsureHostPowerStatus(Guid.Empty.ToString(), "dummy", new HostPowerStatus
        {
            Status = PowerStatus.Unknown,
            Hostname = "dummy"
        });

        Assert.Equal(PowerStatus.Unknown, powerStatus.Status);
    }

    [Fact]
    public async void EnsurePowerStatus()
    {
        IClusterPowerStatusRepository sut = new ClusterPowerStatusRepository(_context, _mapper);

        var powerStatus = await sut.EnsureHostPowerStatus(Guid.Empty.ToString(), "dummy", new HostPowerStatus
        {
            Status = PowerStatus.Unknown,
            Hostname = "dummy"
        });

        Assert.NotNull(powerStatus);
    }


    [Fact]
    public async void GetPowerStatus()
    {
        IClusterPowerStatusRepository sut = new ClusterPowerStatusRepository(_context, _mapper);

        var powerStatus = await sut.GetPowerStatus(docId);

        Assert.NotNull(powerStatus);
    }
}