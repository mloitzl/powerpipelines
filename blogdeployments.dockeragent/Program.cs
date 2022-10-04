using System.Reflection;
using blogdeployments.dockeragent;
using blogdeployments.domain.Events;
using blogdeployments.events;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RabbitMqConfiguration>(builder.Configuration.GetSection("RabbitMQ"));

builder.Services.Configure<ContainerConfig>(builder.Configuration);

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

builder.Services.AddHostedService<QueueListener<PowerOnRequested, StartContainers>>();
builder.Services.AddHostedService<QueueListener<PowerOffRequested, StopContainers>>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Console.WriteLine(builder.Configuration.GetDebugView());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();