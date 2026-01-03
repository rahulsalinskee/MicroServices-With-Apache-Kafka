using Serilog;
using Serilog.Events;

namespace Application.Logger.Logger
{
    public static class LogConfiguration
    {
        private static string _productProjectName = "Product.API";
        private static string _productApiDirLog = "ProductLog";
        private static string _productLogFileName = "ProductLog-.txt";

        private static string _orderProjectName = "Order.API";
        private static string _orderApiDirLog = "OrderLog";
        private static string _orderLogFileName = "OrderLog-.txt";

        public static ILogger GenetateProductLog()
        {
            var logFilePathForProductApi = GetFilePath(logApiDirName: _productApiDirLog, logFileName: _productLogFileName);

            return CreateLog(logPath: logFilePathForProductApi, projectName: _productProjectName);
        }

        private static ILogger GenetateOrderLog()
        {
            var logFilePathForProductApi = GetFilePath(logApiDirName: _orderApiDirLog, logFileName: _orderLogFileName);

            return CreateLog(logPath: logFilePathForProductApi, projectName: _orderProjectName);
        }

        private static string GetFilePath(string logApiDirName, string logFileName)
        {
            var solutionRoot = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..");
            var logDirectory = Path.Combine(solutionRoot, "Application.Logger", "Logs", logApiDirName);
            Directory.CreateDirectory(logDirectory);
            var logPath = Path.Combine(logDirectory, logFileName);

            return logPath;
        }

        private static ILogger CreateLog(string logPath, object projectName)
        {
            return new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithProperty(name: "ApplicationName", value: projectName)
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
