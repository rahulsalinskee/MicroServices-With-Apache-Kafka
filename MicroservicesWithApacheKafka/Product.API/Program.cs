using Application.Exception.GlobalException;
using Application.Logger.Logger;
using ApplicationDataContext.DataBaseConfiguration;
using ApplicationDataContext.DataBaseContext;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Product.API.ProductRepository;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

/* Load configuration from data layer embedded resource. 
*  This forces the API to load the config file embedded inside your library 
*/
ApplicationDataBaseConfiguration.LoadConfiguration(builder: builder.Configuration);

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

    /* Configure the producer */
    var producerConfiguration = new ProducerConfig()
    {
        BootstrapServers = "localhost:9092",
    };

    /* Register the producer's configuration */
    builder.Services.AddSingleton<IProducer<Null, string>>(producer => new ProducerBuilder<Null, string>(config: producerConfiguration).Build());

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
