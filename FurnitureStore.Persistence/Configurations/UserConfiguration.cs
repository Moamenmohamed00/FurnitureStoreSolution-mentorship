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
    public class UserConfiguration: IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Configuration code for User entity goes here
            builder.HasKey(u => u.Id);
            builder.Property(u => u.UserName).IsRequired().HasMaxLength(100);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(200)/*.HasColumnType("Email")*/ ;
            builder.Property(u=>u.PhoneNumber).HasMaxLength(15)/*.HasColumnType("PhoneNumber")*/;
            builder.Property(u => u.CreatedAt).HasConversion<DateTime>();

            builder.HasMany(u => u.Addresses)
                   .WithOne(a => a.User)
                   .HasForeignKey(a => a.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Orders)
                   .WithOne(o => o.User)
                   .HasForeignKey(o => o.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.CartItems)
                   .WithOne(ci => ci.User)
                   .HasForeignKey(ci => ci.UserId)
             .OnDelete(DeleteBehavior.Restrict);
            // في حالة فيه علاقة أن الـ User يقدر يضيف Products
            builder.HasMany(u => u.ProductsCreated)
                .WithOne(p => p.CreatedBy)
                .HasForeignKey(p => p.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
            //admin
            //builder.HasMany(u => u.PurchasedProducts)
            //    .WithOne(p => p.User)
            //    .HasForeignKey(p => p.UserId)
            //    .OnDelete(DeleteBehavior.Restrict);
            //Restrict لو مش عايز المسح يتسلسل
            //(لما تمسح User يتمسح كل Orders و Products) cascade
        }
    }
}
