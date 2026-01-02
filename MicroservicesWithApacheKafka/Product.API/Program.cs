using Application.Exception.GlobalException;
using ApplicationDataContext.DataBaseContext;
using Microsoft.EntityFrameworkCore;
using Product.API.ProductRepository;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Resolve the correct log path relative to solution root
var solutionRoot = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..");
var logDirectory = Path.Combine(solutionRoot, "Application.Logger", "Logs", "ProductLog");
Directory.CreateDirectory(logDirectory);
var logPath = Path.Combine(logDirectory, "ProductLog-.txt");

var productLogger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("ApplicationName", "Product.API")
    .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(
        path: logPath,
        rollingInterval: RollingInterval.Day,
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{ApplicationName}] {Message:lj}{NewLine}{Exception} \n",
        retainedFileCountLimit: 7)
    .CreateLogger();

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
