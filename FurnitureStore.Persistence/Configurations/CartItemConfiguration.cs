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
    public class CartItemConfiguration: IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            // Configuration code for CartItem entity goes here
            builder.HasKey(ci => ci.Id);
            builder.Property(ci => ci.Quantity).IsRequired();
           // builder.Property(ci => ci.AddedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
            builder.HasOne(ci => ci.User)
                   .WithMany(u => u.CartItems)
                   .HasForeignKey(ci => ci.UserId)
              .OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Cascade);

            builder.HasOne(ci => ci.Product)
                   .WithMany(p => p.CartItems)
                   .HasForeignKey(ci => ci.ProductId)
              .OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Restrict);
        }
    }
}
