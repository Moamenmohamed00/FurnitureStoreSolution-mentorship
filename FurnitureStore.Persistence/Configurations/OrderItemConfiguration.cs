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
    public class OrderItemConfiguration: IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            // Configuration code for OrderItem entity goes here
            builder.HasKey(oi => oi.Id);
            builder.Property(oi => oi.Quantity).IsRequired();
            builder.Property(oi => oi.UnitPrice).IsRequired().HasColumnType("decimal(18,2)");
            // builder.Property(oi => oi.TotalPrice).IsRequired().HasColumnType("decimal(18,2)");//computed in the entity
            builder.Property(oi => oi.OrderStatus).IsRequired().HasConversion<string>().HasMaxLength(50);
            builder.HasOne(oi => oi.Order)
                   .WithMany(o => o.OrderItems)
                   .HasForeignKey(oi => oi.OrderId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(oi => oi.Product)
                   .WithMany(p => p.OrderItems)
                   .HasForeignKey(oi => oi.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
