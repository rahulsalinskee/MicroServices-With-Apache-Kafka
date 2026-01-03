using Serilog;
using Serilog.Events;

namespace Application.Logger.Logger
{
    public static class LogConfiguration
    {
        public static ILogger CreateProductLogger()
        {
            var solutionRoot = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..");
            var logDirectory = Path.Combine(solutionRoot, "Application.Logger", "Logs", "ProductLog");
            Directory.CreateDirectory(logDirectory);
            var logPath = Path.Combine(logDirectory, "ProductLog-.txt");

            return new LoggerConfiguration()
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
        }
    }
}
