using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Data.Models;

namespace WarehouseManagement.Data
{
    public class WarehouseDbContext : DbContext
    {
        public WarehouseDbContext(DbContextOptions<WarehouseDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ProductType> productTypes { get; set; }

        public DbSet<DeliveryOption> deliveryOptions { get; set; }
    }
}
