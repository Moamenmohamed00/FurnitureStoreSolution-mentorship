using FurnitureStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FurnitureStore.Persistence.Configurations
{
    public class ProductConfiguration: IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            // Configuration code for Product entity goes here
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
            builder.Property(p => p.Description).HasMaxLength(1000);
            builder.Property(p => p.Price).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(p => p.Stock).IsRequired();
            builder.Property(p => p.Color).IsRequired().HasConversion<string>(); // enum → string
            builder.Property(p => p.Status).IsRequired().HasConversion<string>(); // enum → string

            builder.HasOne(p => p.Category)
                   .WithMany(c => c.Products)
                   .HasForeignKey(p => p.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(p => p.Brand)
                .WithMany(b => b.Products)
                     .HasForeignKey(p => p.BrandId)
            .OnDelete(DeleteBehavior.Restrict);

            // العلاقة مع CreatedBy (Admin/Seller)
            builder.HasOne(p => p.CreatedBy)
                   .WithMany(u => u.ProductsCreated)
                   .HasForeignKey(p => p.CreatedByUserId)
                   .OnDelete(DeleteBehavior.Restrict);

            // العلاقة مع User (Customer/Purchaser)
            //builder.HasOne(p => p.User)
            //       .WithMany(u => u.PurchasedProducts)
            //       .HasForeignKey(p => p.UserId)
            //       .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
