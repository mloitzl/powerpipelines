using System.Reflection;
using blogdeployments.domain;
using blogdeployments.domain.Events;
using blogdeployments.events;
using blogdeployments.events.Sender;
using blogdeployments.instrumentation;
using blogdeployments.repository;
using blogdeployments.ui;
using blogdeployments.ui.Extensions;
using blogdeployments.ui.Handler;
using CouchDB.Driver.DependencyInjection;
using MediatR;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// add OTLP Logging
builder.Logging.AddOpenTelemetry("powerui", "1.0");

// add OTLP Tracing
builder.Services.AddOpenTelemetry("powerui", "1.0", nameof(EventSender));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});


builder.Services.Configure<UserInterfaceConfiguration>(builder.Configuration.GetSection("UserInterfaceConfiguration"));

var couchDbHost =
    $"{builder.Configuration["couchdb:proto"]}://{builder.Configuration["couchdb:host"]}:{builder.Configuration["couchdb:port"]}";

builder.Services.AddCouchContext<DeploymentsContext>(optionBuilder => optionBuilder
    .UseEndpoint(couchDbHost)
    .EnsureDatabaseExists()
    .UseBasicAuthentication(
        builder.Configuration["couchdb:user"],
        builder.Configuration["couchdb:password"]));

builder.Services.AddSingleton<IClusterPowerStatusRepository, ClusterPowerStatusRepository>();

builder.Services.AddSingleton<ServerSentEventsService>();

builder.Services.AddHostedService<QueueListener<PowerOnCompleted, SendHostNotification>>();
builder.Services.AddHostedService<QueueListener<ShutdownCompleted, SendHostNotification>>();
builder.Services.AddHostedService<QueueListener<ClusterIsUp, SendClusterNotification>>();
builder.Services.AddHostedService<QueueListener<ClusterIsDown, SendClusterNotification>>();

builder.Services.AddTransient<IEventSender<PowerOnRequested>, PowerOnRequestedEventSender>();
builder.Services.AddTransient<IEventSender<ShutdownRequested>, ShutdownRequestedEventSender>();

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly(),
    typeof(IClusterPowerStatusRepository).Assembly);

builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    
}

builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

app.UseServerSentEvents();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();
