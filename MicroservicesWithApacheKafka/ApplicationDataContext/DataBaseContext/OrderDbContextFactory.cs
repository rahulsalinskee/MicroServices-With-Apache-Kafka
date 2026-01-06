using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ApplicationDataContext.DataBaseContext
{
    public class OrderDbContextFactory : IDesignTimeDbContextFactory<OrderDbContext>
    {
        public OrderDbContext CreateDbContext(string[] args)
        {
            var currentDirectory = Directory.GetCurrentDirectory();

            var basePath = Path.Combine(currentDirectory, "..", "Order.API");

            if (!Directory.Exists(basePath))
            {
                if (File.Exists(Path.Combine(currentDirectory, "appsettings.json")))
                {
                    basePath = currentDirectory;
                }
                else
                {
                    basePath = Path.Combine(currentDirectory, "Order.API");
                }
            }

            Console.WriteLine($"[Factory] Looking for appsettings.json in: {Path.GetFullPath(basePath)}");

            var configuration = new ConfigurationBuilder().SetBasePath(Path.GetFullPath(basePath)).AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

            var optionsBuilder = new DbContextOptionsBuilder<OrderDbContext>();

            var connectionString = configuration.GetConnectionString("OrderDbConnectionString");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException($"Connection string 'OrderDbConnectionString' not found in {Path.Combine(basePath, "appsettings.json")}.");
            }

            optionsBuilder.UseSqlServer(connectionString);

            return new OrderDbContext(optionsBuilder.Options);
        }
    }
}