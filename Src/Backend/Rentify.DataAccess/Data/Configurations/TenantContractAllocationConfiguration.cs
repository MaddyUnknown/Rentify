using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rentify.Core.Entities;

namespace Rentify.DataAccess.Data.Configurations;

public class TenantContractAllocationConfiguration : IEntityTypeConfiguration<TenantContractAllocation>
{
    public void Configure(EntityTypeBuilder<TenantContractAllocation> builder)
    {
        builder.HasMany(tca => tca.Bills)
            .WithOne()
            .HasForeignKey(b => b.TenantContractAllocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(tca => tca.ContractRentBreakDowns)
            .WithOne()
            .HasForeignKey(rbd => rbd.TenantContractAllocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(tca => tca.Deposits)
            .WithOne()
            .HasForeignKey(d => d.TenantContractAllocationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}