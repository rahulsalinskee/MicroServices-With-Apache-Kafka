using Application.Exception.GlobalException;
using ApplicationDataContext.DataBaseContext;
using Microsoft.EntityFrameworkCore;
using Product.API.ProductRepository;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var productLogger = new LoggerConfiguration().WriteTo.File(path: "MicroservicesWithApacheKafka/Application.Logger/Logs/ProductLog.ProductLog.txt", rollingInterval: RollingInterval.Day).MinimumLevel.Information().CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger: productLogger);

builder.Services.AddDbContext<ProductDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString(name: "ProductDbConnectionString"));
});

builder.Services.AddScoped<IProductService, ProductImplementation>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    /* Swagger UI for Product API is loaded here */
    app.UseSwaggerUI(option =>
    {
        option.SwaggerEndpoint(url: "/openapi/v1.json", name: "Product API");
    });
}

app.UseMiddleware<GlobalExceptionHandler>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
