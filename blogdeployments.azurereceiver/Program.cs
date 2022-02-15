using blogdeployments.azurereceiver;
using blogdeployments.azurereceiver.BackgroundService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.Configure<AzureQueue>(builder.Configuration.GetSection("azurequeue"));

builder.Services.AddHostedService<AzureQueueReceiver>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
