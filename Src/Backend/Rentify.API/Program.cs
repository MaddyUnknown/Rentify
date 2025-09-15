using Microsoft.EntityFrameworkCore;
using Rentify.DataAccess.Data;
using Rentify.DataAccess.Repositories;
using Rentify.DataAccess.UnitOfWork;
using Rentify.Service.Interfaces;
using Rentify.Service.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add DbContext
builder.Services.AddDbContext<RentifyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Add repository pattern
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Add application services
builder.Services.AddScoped<IPropertyService, PropertyService>();
builder.Services.AddScoped<IUnitService, UnitService>();
builder.Services.AddScoped<IUtilityService, UtilityService>();

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
