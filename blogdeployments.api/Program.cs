using System.Reflection;
using blogdeployments.api;
using blogdeployments.domain;
using blogdeployments.domain.Events;
using blogdeployments.events;
using blogdeployments.events.Sender;
using blogdeployments.handler;
using blogdeployments.instrumentation;
using blogdeployments.repository;
using CouchDB.Driver.DependencyInjection;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;

var sharePointUri = "sharePointUri";

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine(builder.Configuration.GetDebugView());

builder.Logging.AddOpenTelemetry("api", "1.0");

// Add services to the container.

builder.Services.AddOpenTelemetry("api", "1.0",
    nameof(EventSender));

builder.Services.AddControllers();
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddCors(options =>
{
    options.AddPolicy(sharePointUri,
        builder =>
        {
            builder.WithOrigins(
                    "https://*.sharepoint.com",
                    "https://localhost:7099",
                    "http://localhost:3000",
                    "https://*.web.core.windows.net/")
                .SetIsOriginAllowedToAllowWildcardSubdomains()
                .AllowCredentials()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

IdentityModelEventSource.ShowPII = true;
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration);

builder.Services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    // The claim in the Jwt token where App roles are available.
    options.TokenValidationParameters.RoleClaimType = "roles";
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        AuthorizationPolicies.ConfigManageRequired,
        policy => policy.RequireRole(AppRole.AppRoleConfigManage)
    );
});

var couchDbHost =
    $"{builder.Configuration["couchdb:proto"]}://{builder.Configuration["couchdb:host"]}:{builder.Configuration["couchdb:port"]}";

builder.Services.AddCouchContext<DeploymentsContext>(optionBuilder => optionBuilder
    .UseEndpoint(couchDbHost)
    .EnsureDatabaseExists()
    .UseBasicAuthentication(
        builder.Configuration["couchdb:user"],
        builder.Configuration["couchdb:password"]));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
    {
        options.EnableAnnotations();
        options.AddSecurityDefinition(
            "oauth2", new OpenApiSecurityScheme
            {
                Description = "Oauth2 client credentials",
                Name = "Authorization",
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    /*AuthorizationCode = new OpenApiOAuthFlow()
                    {
                        AuthorizationUrl = new Uri("https://login.microsoftonline.com/60df2466-a102-404a-8d9d-95c950626730/oauth2/v2.0/authorize"),
                        TokenUrl = new Uri("https://login.microsoftonline.com/60df2466-a102-404a-8d9d-95c950626730/oauth2/v2.0/token"),
                        Scopes = new Dictionary<string, string>
                        {
                            { "api://com.loitzl.test/blogdeployments/.default", "Reads the Weather forecast" }
                        }
                      
                    },
                    ClientCredentials = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri("https://login.microsoftonline.com/60df2466-a102-404a-8d9d-95c950626730/oauth2/v2.0/authorize"),
                        TokenUrl = new Uri("https://login.microsoftonline.com/60df2466-a102-404a-8d9d-95c950626730/oauth2/v2.0/token"),
                        Scopes = new Dictionary<string, string>
                        {
                            { "api://com.loitzl.test/blogdeployments/.default", "Reads the Weather forecast" }
                        }
                    }*/
                }
            }
        );
    }
);

builder.Services.AddMediatR(
    Assembly.GetExecutingAssembly(),
    typeof(CreateDeployment).Assembly);

builder.Services.AddAutoMapper(
    Assembly.GetExecutingAssembly(),
    typeof(IDeploymentsRepository).Assembly,
    typeof(CreateDeployment.CreateDeploymentHandler).Assembly);

builder.Services.Configure<RabbitMqConfiguration>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddTransient<IEventSender<PowerOnRequested>, PowerOnRequestedEventSender>();
builder.Services.AddTransient<IEventSender<ShutdownRequested>, ShutdownRequestedEventSender>();

builder.Services.AddSingleton<IDeploymentsRepository, DeploymentsRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseCors(sharePointUri);

app.MapControllers();

app.Run();