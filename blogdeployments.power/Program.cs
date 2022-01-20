using System.Reflection;
using blogdeployments.domain;
using blogdeployments.domain.Events;
using blogdeployments.events;
using blogdeployments.events.EventSender;
using blogdeployments.power;
using blogdeployments.power.Handler;
using blogdeployments.power.Service;
using blogdeployments.repository;
using CouchDB.Driver.DependencyInjection;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

var couchDbHost =
    $"{builder.Configuration["couchdb:proto"]}://{builder.Configuration["couchdb:host"]}:{builder.Configuration["couchdb:port"]}";

builder.Services.AddCouchContext<DeploymentsContext>(optionBuilder => optionBuilder
    .UseEndpoint(couchDbHost)
    .EnsureDatabaseExists()
    .UseBasicAuthentication(
        builder.Configuration["couchdb:user"],
        builder.Configuration["couchdb:password"]));


builder.Services.Configure<RabbitMqConfiguration>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.Configure<RaspbeeConfiguration>(builder.Configuration.GetSection("Raspbee"));
builder.Services.Configure<ClusterTopologyConfiguration>(builder.Configuration.GetSection("ClusterTopology"));

builder.Services.AddHostedService<QueueListener<PowerOnRequested, PowerOn>>();
builder.Services.AddHostedService<QueueListener<ShutdownInitiated, CheckHostStatus>>();
builder.Services.AddHostedService<QueueListener<PowerOnCompleted, UpdatePowerStatus>>();
builder.Services.AddTransient<IEventSender<ShutdownInitiated>, ShutdownInitiatedEventSender>();

builder.Services.AddSingleton<IClusterPowerStatusRepository, ClusterPowerStatusRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly(),
    typeof(IClusterPowerStatusRepository).Assembly);

builder.Services.AddTransient<IRaspbeeService, RaspbeeService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();