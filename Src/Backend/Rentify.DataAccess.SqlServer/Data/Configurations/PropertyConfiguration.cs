using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rentify.Core.Entities;
using Rentify.Core.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.DataAccess.SqlServer.Data.Configurations
{
    public class PropertyConfiguration : IEntityTypeConfiguration<Property>
    {
        public void Configure(EntityTypeBuilder<Property> builder)
        {
            builder.OwnsOne(p => p.PropertyAddress);

            builder.OwnsOne(p => p.PropertyLocation, location =>
            {
                location.Property(l => l.Latitude).HasPrecision(9, 6);
                location.Property(l => l.Longitude).HasPrecision(9, 6);
            });

            builder.HasOne(p => p.Owner)
                .WithMany()
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.RequestedCoverPic)
                .WithOne()
                .HasForeignKey<Property>(p => p.RequestedCoverPicId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(p => p.ActiveCoverPic)
                .WithOne()
                .HasForeignKey<Property>(p => p.ActiveCoverPicId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
