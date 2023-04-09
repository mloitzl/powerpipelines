using System.Reflection;
using blogdeployments.agent;
using blogdeployments.agent.BackgroundService;
using blogdeployments.agent.Handler;
using blogdeployments.domain;
using blogdeployments.domain.Events;
using blogdeployments.events;
using blogdeployments.events.Sender;
using blogdeployments.instrumentation;
using MediatR;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine(builder.Configuration.GetDebugView());

// add OTLP Logging
builder.Logging.AddOpenTelemetry("agent", "1.0");

// add OTLP Tracing
builder.Services.AddOpenTelemetry("agent", "1.0",
    nameof(EventSender), nameof(QueueListenerBackgroundService));

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddApplicationInsightsTelemetry();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var agentConfiguration = new AgentConfiguration();
builder.Configuration.GetSection("Agent").Bind(agentConfiguration);

agentConfiguration.RunningInContainer = builder.Configuration.GetValue<bool>("DOTNET_RUNNING_IN_CONTAINER");

builder.Services.AddSingleton(Options.Create(agentConfiguration));

// Messaging
builder.Services.Configure<RabbitMqConfiguration>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddTransient<IEventSender<PowerOnCompleted>, PowerOnCompletedEventSender>();
builder.Services.AddTransient<IEventSender<ShutdownInitiated>, ShutdownInitiatedEventSender>();
builder.Services.AddHostedService<QueueListener<ShutdownRequested, Shutdown>>();

// HostedService
builder.Services.AddHostedService<StartupService>();

// Mapping
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