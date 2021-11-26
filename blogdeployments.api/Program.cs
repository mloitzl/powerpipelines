using System.Reflection;
using blogdeployments.api;
using blogdeployments.api.Sender;
using blogdeployments.repository;
using CouchDB.Driver.DependencyInjection;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Logging;

string sharePointUri = "sharePointUri";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCors(options =>
                    {
                        options.AddPolicy(name: sharePointUri,
                                    builder =>
                                    {
                                        builder.WithOrigins("https://*.sharepoint.com")
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

builder.Services.AddCouchContext<DeploymentsContext>(optionBuilder => optionBuilder
    .UseEndpoint("http://localhost:5984/")
    .EnsureDatabaseExists()
    .UseBasicAuthentication(
        username: builder.Configuration["couchdb_user"],
        password: builder.Configuration["couchdb_password"]));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddAutoMapper(
    Assembly.GetExecutingAssembly(), 
    typeof(IDeploymentsRepository).Assembly);

builder.Services.Configure<RabbitMqConfiguration>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddTransient<IRegisterDeploymentSender, RegisterDeploymentSender>();
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
