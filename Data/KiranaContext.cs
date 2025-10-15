using Microsoft.EntityFrameworkCore;
using KiranaStore.Models;

namespace KiranaStore.Data
{
    public class KiranaContext : DbContext
    {
        public KiranaContext(DbContextOptions<KiranaContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }

    }
}
