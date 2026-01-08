using Microsoft.EntityFrameworkCore;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationDataContext.DataBaseContext
{
    public class OrderSummaryDbContext : DbContext
    {
        public OrderSummaryDbContext(DbContextOptions<OrderSummaryDbContext> options) : base(options)
        {
        }

        public DbSet<OrderSummary> OrderSummaries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
