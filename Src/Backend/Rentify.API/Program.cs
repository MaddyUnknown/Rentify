using Rentify.Application.Extensions;
using Rentify.DataAccess.SqlServer.Extensions;
using Rentify.Storage.LocalStorage.Extensions;
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

// Add data access services
builder.Services.AddDataAccessServices(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("ApplicationDBConnection");
});

// Add storage services
builder.Services.AddLocalStorageServices(options =>
{
    options.RootFolder = builder.Configuration.GetValue<string>("Storage:RootFolder");
    options.StreamBufferSize = builder.Configuration.GetValue<int>("Storage:StreamBufferSize");
});

// Add application services
builder.Services.AddApplicationServices();

// Add Swagger for development
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Rentify API", Version = "v1" });
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

app.MapControllers();

app.Run();
