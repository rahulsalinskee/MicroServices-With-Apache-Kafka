using Microsoft.EntityFrameworkCore;

namespace ApplicationDataContext.DataBaseContext
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
        {
        }

        public DbSet<Shared.Models.Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the Product entity
            modelBuilder.Entity<Shared.Models.Product>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.Price).HasPrecision(18, 2);
            });
        }
    }
}
