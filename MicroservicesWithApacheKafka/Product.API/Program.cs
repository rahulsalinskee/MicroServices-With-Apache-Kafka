using Application.Exception.GlobalException;
using Application.Logger.Logger;
using ApplicationDataContext.DataBaseContext;
using Microsoft.EntityFrameworkCore;
using Product.API.ProductRepository;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var productLogger = LogConfiguration.GenetateProductLog();

Log.Logger = productLogger;

try
{
    builder.Logging.ClearProviders();
    builder.Logging.AddSerilog(logger: productLogger);

    builder.Services.AddDbContext<ProductDbContext>(option =>
    {
        option.UseSqlServer(builder.Configuration.GetConnectionString(name: "ProductDbConnectionString"));
    });

    builder.Services.AddScoped<IProductService, ProductImplementation>();

    builder.Services.AddControllers();
    builder.Services.AddOpenApi();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
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
}
catch (System.Exception exception)
{
    Log.Fatal(exception, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
