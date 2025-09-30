using FurnitureStore.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FurnitureStore.Persistence.DbContext
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            /*// Apply configurations for each entity
            new Configurations.ProductConfiguration().Configure(modelBuilder.Entity<Product>());
            new Configurations.CategoryConfiguration().Configure(modelBuilder.Entity<Category>());
            new Configurations.BrandConfiguration().Configure(modelBuilder.Entity<Brand>());
            new Configurations.UserConfiguration().Configure(modelBuilder.Entity<User>());
            new Configurations.AddressConfiguration().Configure(modelBuilder.Entity<Address>());
            new Configurations.OrderConfiguration().configure(modelBuilder.Entity<Order>());
            new Configurations.OrderItemConfiguration().Configure(modelBuilder.Entity<OrderItem>());
            new Configurations.CartItemConfiguration().Configure(modelBuilder.Entity<CartItem>());*/
            // Add configurations for other entities as needed
            // Load all configurations automatically
            // modelBuilder.ApplyConfigurationsFromAssembly( Assembly.GetExecutingAssembly());
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
        // DbSets for each entity
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<RefreshToken> RefreshTokens
        {
            get; set;
        }
    }
}