using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;
using blogdeployments.domain;
using CouchDB.Driver;
using CouchDB.Driver.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;
using Xunit;

namespace blogdeployments.repository.tests;

[Trait("Category", "Integration")]
[assembly: UserSecretsId("blogdeployments.repository.tests")]
public class UnitTest1
{
    private readonly string docId = "7b018418-b99e-4e8f-980b-327fba932a4e";
    private readonly IMapper _mapper;
    private readonly DeploymentsContext _context;

    public UnitTest1()
    {
        var builder = new ConfigurationBuilder()
            .AddUserSecrets(Assembly.GetExecutingAssembly(), true);
        
        var configuration = builder.Build();
        
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new  blogdeployments.repository.Model.MappingProfile());
        });
        
        _mapper = mappingConfig.CreateMapper();
        CouchOptionsBuilder<DeploymentsContext> couchOptionsBuilder = new CouchOptionsBuilder<DeploymentsContext>();
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

        var powerStatus = await sut.EnsurePowerStatus(new ClusterPowerStatus
        {
            PowerRequestId = Guid.NewGuid(),
            Id = docId,
            HostsPower = new Dictionary<string, HostPowerStatus>
            {
                {"dummy", new HostPowerStatus
                {
                    Status = PowerStatus.Unknown,
                    Hostname = "dummy"
                }}
            }
        });
        
        Assert.Equal(PowerStatus.Unknown, powerStatus.HostsPower.First().Value.Status);
    }

    
    [Fact]
    public async void PowerOnStatus()
    {
        IClusterPowerStatusRepository sut = new ClusterPowerStatusRepository(_context, _mapper);

        var powerStatus = await sut.EnsurePowerStatus(new ClusterPowerStatus
        {
            PowerRequestId = Guid.NewGuid(),
            Id = docId,
            HostsPower = new Dictionary<string, HostPowerStatus>
            {
                {"dummy", new HostPowerStatus
                {
                    Status = PowerStatus.On,
                    Hostname = "dummy"
                }}
            }
        });
        
        Assert.Equal(PowerStatus.On, powerStatus.HostsPower.First().Value.Status);
    }

    [Fact]
    public async void EnsurePowerStatus()
    {
        IClusterPowerStatusRepository sut = new ClusterPowerStatusRepository(_context, _mapper);

        var powerStatus = await sut.EnsurePowerStatus(new ClusterPowerStatus
        {
            PowerRequestId = Guid.NewGuid(),
            Id = docId,
            HostsPower = new Dictionary<string, HostPowerStatus>
            {
                {"dummy", new HostPowerStatus
                {
                    Status = PowerStatus.Unknown,
                    Hostname = "dummy"
                }}
            }
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

    
    [Fact]
    public async void AddPowerStatus()
    {
        IClusterPowerStatusRepository sut = new ClusterPowerStatusRepository(_context, _mapper);

        var powerStatus = await sut.AddPowerStatus(new ClusterPowerStatus
        {
            Id = docId,
            HostsPower = new Dictionary<string, HostPowerStatus>
            {
                {"dummy", new HostPowerStatus
                {
                    Status = PowerStatus.Unknown,
                    Hostname = "dummy"
                }}
            },
            PowerRequestId = Guid.NewGuid()
        });
        
        Assert.NotNull(powerStatus);
    }
}