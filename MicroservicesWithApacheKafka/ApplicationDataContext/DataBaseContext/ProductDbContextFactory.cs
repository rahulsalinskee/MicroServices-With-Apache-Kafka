using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ApplicationDataContext.DataBaseContext
{
    public class ProductDbContextFactory : IDesignTimeDbContextFactory<ProductDbContext>
    {
        public ProductDbContext CreateDbContext(string[] args)
        {
            // 1. Get the current directory (where the command is running)
            var currentDirectory = Directory.GetCurrentDirectory();

            // 2. Define the path to the API project (Product.API)
            // We use Path.Combine with separate arguments to ensure cross-platform compatibility (Windows/Linux/Mac)
            // We assume the structure is:
            // SolutionFolder/
            //   |__ Product.API/
            //   |__ ApplicationDataContext/
            var basePath = Path.Combine(currentDirectory, "..", "Product.API");

            // 3. Fallback: If running from the Solution root or API folder directly, adjust logic
            if (!Directory.Exists(basePath))
            {
                // If the computed path doesn't exist, we might already be in the API folder or Solution root
                // Let's try to find appsettings.json in the current directory first
                if (File.Exists(Path.Combine(currentDirectory, "appsettings.json")))
                {
                    basePath = currentDirectory;
                }
                else
                {
                    // If we are in the ApplicationDataContext folder, the first path calculation should have worked.
                    // If we are in Solution Root, we need to go down into Product.API
                    basePath = Path.Combine(currentDirectory, "Product.API");
                }
            }

            Console.WriteLine($"[Factory] Looking for appsettings.json in: {Path.GetFullPath(basePath)}");

            var configuration = new ConfigurationBuilder().SetBasePath(Path.GetFullPath(basePath)).AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

            var optionsBuilder = new DbContextOptionsBuilder<ProductDbContext>();

            var connectionString = configuration.GetConnectionString("ProductDbConnectionString");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException($"Connection string 'ProductDbConnectionString' not found in {Path.Combine(basePath, "appsettings.json")}.");
            }

            optionsBuilder.UseSqlServer(connectionString);

            return new ProductDbContext(optionsBuilder.Options);
        }
    }
}
