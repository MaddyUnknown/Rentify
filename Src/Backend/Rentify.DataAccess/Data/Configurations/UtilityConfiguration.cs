using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rentify.Core.Entities;

namespace Rentify.DataAccess.Data.Configurations;

public class UtilityConfiguration : IEntityTypeConfiguration<Utility>
{
    public void Configure(EntityTypeBuilder<Utility> builder)
    {
        builder.HasOne(u => u.Property)
            .WithMany()
            .HasForeignKey(u => u.PropertyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}