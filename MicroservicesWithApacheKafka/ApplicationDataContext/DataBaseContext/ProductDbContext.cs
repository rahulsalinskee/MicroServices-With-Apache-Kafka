using Microsoft.EntityFrameworkCore;

namespace ApplicationDataContext.DataBaseContext
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
        {
        }

        public DbSet<Shared.Models.Product> Products { get; set; }
    }
}
