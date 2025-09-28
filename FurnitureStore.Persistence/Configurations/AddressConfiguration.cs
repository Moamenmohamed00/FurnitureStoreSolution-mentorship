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
    public class AddressConfiguration: IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            // Configuration code for Address entity goes here
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Street).IsRequired().HasMaxLength(200);
            builder.Property(a => a.City).IsRequired().HasMaxLength(100);
            builder.Property(a => a.State).IsRequired().HasMaxLength(100);
            builder.Property(a => a.ZipCode).HasMaxLength(20);
            builder.Property(a => a.Country).IsRequired().HasConversion<string>();// enum → string
            builder.HasOne(a => a.User)
                   .WithMany(u => u.Addresses)
                   .HasForeignKey(a => a.UserId)
                   .OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Cascade);
        }
    }
}
