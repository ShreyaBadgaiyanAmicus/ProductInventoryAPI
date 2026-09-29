using Microsoft.EntityFrameworkCore;
using ProductInventoryAPI.Models;


namespace ProductInventoryAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { 


            }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.Suppliers)
                .WithMany(s => s.Products);

            modelBuilder.Entity<Supplier>().HasData(
    new Supplier
    {
        Id = 1,
        Name = "ABC Electronics",
        ContactEmail = "abc@example.com",
        Phone = "9876543210"
    },
    new Supplier
    {
        Id = 2,
        Name = "Tech Supplies",
        ContactEmail = "tech@example.com",
        Phone = "9876543211"
    },
    new Supplier
    {
        Id = 3,
        Name = "Global Components",
        ContactEmail = "global@example.com",
        Phone = "9876543212"
    }
);
        }

        public DbSet<ProductInventoryUser> ProductInventoryUsers { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }


    }
}
