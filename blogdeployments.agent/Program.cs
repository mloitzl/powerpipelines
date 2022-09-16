using System.Reflection;
using blogdeployments.agent;
using blogdeployments.agent.BackgroundService;
using blogdeployments.agent.Handler;
using blogdeployments.domain;
using blogdeployments.domain.Events;
using blogdeployments.events;
using blogdeployments.events.EventSender;
using MediatR;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine(builder.Configuration.GetDebugView());

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApplicationInsightsTelemetry();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var agentConfiguration = new AgentConfiguration();
builder.Configuration.GetSection("Agent").Bind(agentConfiguration);
agentConfiguration.RunningInContainer = builder.Configuration.GetValue<bool>("RUNNING_IN_CONTAINER");
builder.Services.AddSingleton(Options.Create(agentConfiguration));

builder.Services.Configure<RabbitMqConfiguration>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddTransient<IEventSender<PowerOnCompleted>, PowerOnCompletedEventSender>();
builder.Services.AddTransient<IEventSender<ShutdownInitiated>, ShutdownInitiatedEventSender>();

builder.Services.AddHostedService<StartupService>();

builder.Services.AddHostedService<QueueListener<ShutdownRequested, Shutdown>>();

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();