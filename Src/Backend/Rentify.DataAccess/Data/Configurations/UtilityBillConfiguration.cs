using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rentify.Core.Entities;

namespace Rentify.DataAccess.Data.Configurations;

public class UtilityBillConfiguration : IEntityTypeConfiguration<UtilityBill>
{
    public void Configure(EntityTypeBuilder<UtilityBill> builder)
    {
        builder.Property(e => e.Amount).HasColumnType("money");
        builder.HasOne(ub => ub.Utility)
            .WithMany()
            .HasForeignKey(ub => ub.UtilityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}