using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace ApplicationDataContext.DataBaseContext
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderSummary> OrdersSummary { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            /* Configure Order entity */
            modelBuilder.Entity<Order>(eachOrder =>
            {
                eachOrder.HasKey(order => order.Id);

                eachOrder.Property(order => order.ProductId).HasMaxLength(50);

                eachOrder.Property(order => order.Quantity).HasPrecision(18, 2);
            });

            /* Configure OrderSummary entity */
            modelBuilder.Entity<OrderSummary>(eachOrderSummary =>
            {
                eachOrderSummary.HasKey(orderSummary => orderSummary.OrderId);

                eachOrderSummary.Property(orderSummary => orderSummary.ProductName).HasMaxLength(50);

                eachOrderSummary.Property(orderSummary => orderSummary.TotalAmount).HasPrecision(18, 2);
            });
        }
    }
}
