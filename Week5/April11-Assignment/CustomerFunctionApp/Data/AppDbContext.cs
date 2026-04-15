using CustomerFunctionApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerFunctionApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    "Server=LENOVOLEGION5\\SQLEXPRESS;Database=CustomerDB;TrustServerCertificate=True;");
            }
        }
    }
}