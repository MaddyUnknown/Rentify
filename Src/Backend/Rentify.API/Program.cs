using Microsoft.Extensions.Options;
using Rentify.API.Accessors;
using Rentify.API.Middlewares;
using Rentify.API.Swagger.OperationFilters;
using Rentify.Application.Extensions;
using Rentify.Auth.Core.Abstractions.Accessors;
using Rentify.Auth.Identity.Extensions;
using Rentify.Core.Abstractions.Accessors;
using Rentify.DataAccess.SqlServer.Extensions;
using Rentify.FileWorkflow.Implementation.Extensions;
using Rentify.Storage.LocalStorage.Extensions;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower));
});

builder.Services.AddHttpContextAccessor();

// Add services for Rentify.API
builder.Services.AddSingleton<DataScopeAccessor>();
builder.Services.AddSingleton<IDataScopeAccessor>(provider => provider.GetRequiredService<DataScopeAccessor>());
builder.Services.AddSingleton<UserContextAccessor>();
builder.Services.AddSingleton<IUserContextAccessor>(provider => provider.GetRequiredService<UserContextAccessor>());

// Add data access services
builder.Services.AddDataAccessServices(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("ApplicationDBConnection");
});

// Add auth identity services
builder.Services.AddIdentityAuthServices(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("ApplicationDBConnection");
    options.JwtTokenIssuer = builder.Configuration.GetValue<string>("Auth:Issuer");
    options.JwtTokenAudience = builder.Configuration.GetValue<string>("Auth:Audience");
    options.JwtAccessTokenSigningSecret = builder.Configuration.GetValue<string>("Auth:JwtAccessTokenSigningSecret");
    options.JwtAccessTokenDurationInMinutes = builder.Configuration.GetValue<int>("Auth:AccessTokenDurationInMinutes");
    options.JwtRefreshTokenDurationInDays = builder.Configuration.GetValue<int>("Auth:JwtAccessTokenDurationInMinutes");
});

// Add storage services
builder.Services.AddLocalStorageServices(options =>
{
    options.RootFolder = builder.Configuration.GetValue<string>("Storage:RootFolder");
    options.StreamBufferSize = builder.Configuration.GetValue<int>("Storage:StreamBufferSize");
});

// Add file workflow services
builder.Services.AddFileWorkflowServices();

// Add application services
builder.Services.AddApplicationServices();

// Add Swagger for development
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Rentify API", Version = "v1" });

    // Define Bearer token scheme
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter your valid token.\nExample: abc123"
    });

    // Apply globally
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    options.OperationFilter<SubscriptionHeaderOperationFilter>();
});

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("dev", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Rentify API v1");
        c.RoutePrefix = string.Empty; // Make Swagger the default page
    });
}

app.UseHttpsRedirection();

app.UseRouting();

if(app.Environment.IsDevelopment())
{
    app.UseCors("dev");
}

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<UserContextMiddleware>();
app.UseMiddleware<DataScopeMiddleware>();

app.MapControllers();

app.Run();
