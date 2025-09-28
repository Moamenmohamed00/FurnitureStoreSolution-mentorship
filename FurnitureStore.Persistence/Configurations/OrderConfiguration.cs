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
    public class OrderConfiguration: IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            // Configuration code for Order entity goes here
            builder.HasKey(o => o.Id);
            builder.Property(o => o.OrderDate).IsRequired().HasDefaultValueSql("GETUTCDATE()");
            builder.Property(o => o.TotalPrice).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(o => o.ShippingCountry).IsRequired().HasConversion<string>(); // enum → string
            builder.Property(o => o.paymentMethod).IsRequired().HasConversion<string>(); // enum → string
            builder.HasOne(o => o.User)
                   .WithMany(u => u.Orders)
                   .HasForeignKey(o => o.UserId)
             .OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(o => o.OrderItems)
                   .WithOne(oi => oi.Order)
                   .HasForeignKey(oi => oi.OrderId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
