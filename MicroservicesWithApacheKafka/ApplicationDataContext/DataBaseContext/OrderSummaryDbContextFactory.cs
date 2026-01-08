//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Design;
//using Microsoft.Extensions.Configuration;

//namespace ApplicationDataContext.DataBaseContext
//{
//    public class OrderSummaryDbContextFactory : IDesignTimeDbContextFactory<OrderSummaryDbContext>
//    {
//        public OrderSummaryDbContext CreateDbContext(string[] args)
//        {
//            var currentDirectory = Directory.GetCurrentDirectory();

//            var basePath = Path.Combine(currentDirectory, "..", "Order.API");

//            if (!Directory.Exists(basePath))
//            {
//                if (File.Exists(Path.Combine(currentDirectory, "appsettings.json")))
//                {
//                    basePath = currentDirectory;
//                }
//                else
//                {
//                    basePath = Path.Combine(currentDirectory, "Order.API");
//                }
//            }

//            Console.WriteLine($"[Factory] Looking for appsettings.json in: {Path.GetFullPath(basePath)}");

//            /* 1. Build the configuration to read appsettings.json. 
//            *  We point to the Order.API project path to find the correct connection string 
//            */
//            var configuration = new ConfigurationBuilder().SetBasePath(Path.GetFullPath(basePath)).AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

//            /* 2. Create the options builder */
//            var optionsBuilder = new DbContextOptionsBuilder<OrderSummaryDbContext>();

//            /* 3. Get the connection string. 
//            *  MAKE SURE this key "OrderDbConnectionString" matches your appsettings.json exactly.
//            */
//            var connectionString = configuration.GetConnectionString("OrderSummaryDbConnectionString");

//            /* 4. Validate the connection string */
//            if (string.IsNullOrEmpty(connectionString))
//            {
//                throw new InvalidOperationException($"Connection string 'OrderSummaryDbConnectionString' not found in {Path.Combine(basePath, "appsettings.json")}.");
//            }

//            /* 5. Add the connection string to the options builder */
//            optionsBuilder.UseSqlServer(connectionString);

//            /* 6. Create and return the DbContext */
//            return new OrderSummaryDbContext(optionsBuilder.Options);
//        }
//    }
//}


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ApplicationDataContext.DataBaseContext
{
    public class OrderSummaryDbContextFactory : IDesignTimeDbContextFactory<OrderSummaryDbContext>
    {
        public OrderSummaryDbContext CreateDbContext(string[] args)
        {
            var currentDirectory = Directory.GetCurrentDirectory();

            /* 1. CHANGE THIS: Point to "ApplicationDataContext" where your appsettings.json lives.
            *  We assume the structure is sibling folders:
            *  Solution/
            *   |__ Order.API/
            *   |__ ApplicationDataContext/ <--- We want this one
            */
            var basePath = Path.Combine(currentDirectory, "..", "ApplicationDataContext");

            /* 2. Robust Path Logic: If the calculated path doesn't exist, we might be running directly from the solution root or inside the project itself. */
            if (!Directory.Exists(basePath))
            {
                /* Try looking in the current directory (if we are running inside ApplicationDataContext) */
                if (File.Exists(Path.Combine(currentDirectory, "appsettings.json")))
                {
                    basePath = currentDirectory;
                }
                else
                {
                    /* Fallback: Assume we are at Solution Root and need to go down into the folder */
                    basePath = Path.Combine(currentDirectory, "ApplicationDataContext");
                }
            }

            Console.WriteLine($"[Factory] Looking for appsettings.json in: {Path.GetFullPath(basePath)}");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.GetFullPath(basePath))
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<OrderSummaryDbContext>();

            /* 3. Get the connection string (This key exists in ApplicationDataContext/appsettings.json) */
            var connectionString = configuration.GetConnectionString("OrderSummaryDbConnectionString");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException($"Connection string 'OrderSummaryDbConnectionString' not found in {Path.Combine(basePath, "appsettings.json")}.");
            }

            optionsBuilder.UseSqlServer(connectionString);

            return new OrderSummaryDbContext(optionsBuilder.Options);
        }
    }
}