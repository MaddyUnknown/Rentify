using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rentify.Core.Entities;

namespace Rentify.DataAccess.Data.Configurations;

public class ContractRentBreakDownConfiguration : IEntityTypeConfiguration<ContractRentBreakDown>
{
    public void Configure(EntityTypeBuilder<ContractRentBreakDown> builder)
    {
        builder.Property(e => e.RentItemAmount).HasColumnType("money");
        builder.Property(e => e.RentItemPercentage).HasPrecision(6, 3);
        builder.HasOne(crb => crb.RentItemUtility)
            .WithMany()
            .HasForeignKey(crb => crb.UtilityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}