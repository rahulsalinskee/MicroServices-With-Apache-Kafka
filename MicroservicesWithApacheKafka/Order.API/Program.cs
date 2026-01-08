using Application.Exception.GlobalException;
using Application.Logger.Logger;
using ApplicationDataContext.DataBaseConfiguration;
using ApplicationDataContext.DataBaseContext;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Order.API.OrderRespository;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

/* Load configuration from data layer embedded resource. 
*  This forces the API to load the config file embedded inside your library 
*/
ApplicationDataBaseConfiguration.LoadConfiguration(builder: builder.Configuration);

/* Configure Serilog */
var orderLogger = LogConfiguration.GenetateOrderLog();
Log.Logger = orderLogger;

try
{
    builder.Logging.ClearProviders();
    builder.Logging.AddSerilog(logger: orderLogger);

    /* Register order DbContext */
    builder.Services.AddDbContext<OrderDbContext>(option =>
    {
        option.UseSqlServer(builder.Configuration.GetConnectionString(name: "OrderDbConnectionString"));
    });

    /* Register order summary DbContext */
    builder.Services.AddDbContext<OrderSummaryDbContext>(option =>
    {
        option.UseSqlServer(builder.Configuration.GetConnectionString(name: "OrderSummaryDbConnectionString"));
    });

    /* Register the Product DbContext */
    builder.Services.AddDbContext<ProductDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString(name: "ProductDbConnectionString"));
    });

    /* Configure Kafka Producer */
    var kafkaConfig = new ProducerConfig
    {
        BootstrapServers = builder.Configuration["Kafka:BootstrapServers"] ?? "localhost:9092"
    };
    builder.Services.AddSingleton<IProducer<Null, string>>(sp => new ProducerBuilder<Null, string>(kafkaConfig).Build());

    /* Configure Kafka Consumer */
    var consumerConfig = new ConsumerConfig
    {
        GroupId = "add-product-consumer-group",
        AutoOffsetReset = AutoOffsetReset.Earliest,
        BootstrapServers = builder.Configuration["Kafka:BootstrapServers"] ?? "localhost:9092",
    };
    builder.Services.AddSingleton<IConsumer<Null, string>>(sp => new ConsumerBuilder<Null, string>(consumerConfig).Build());

    builder.Services.AddScoped<IOrderService, OrderImplementation>();
    builder.Services.AddControllers();
    builder.Services.AddOpenApi();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.UseSwaggerUI(option =>
        {
            option.SwaggerEndpoint(url: "/openapi/v1.json", name: "Order.API");
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
