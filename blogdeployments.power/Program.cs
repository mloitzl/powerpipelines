using System.Reflection;
using blogdeployments.domain.Events;
using blogdeployments.events;
using blogdeployments.power;
using blogdeployments.power.Handler;
using blogdeployments.power.Service;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<RabbitMqConfiguration>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.Configure<RaspbeeConfiguration>(builder.Configuration.GetSection("Raspbee"));

builder.Services.AddHostedService<QueueListener<PowerOnRequested, PowerOn>>();
builder.Services.AddHostedService<QueueListener<ShutdownInitiated, CheckHostStatus>>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

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