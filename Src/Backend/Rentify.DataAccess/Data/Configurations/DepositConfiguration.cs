using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rentify.Core.Entities;

namespace Rentify.DataAccess.Data.Configurations;

public class DepositConfiguration : IEntityTypeConfiguration<Deposit>
{
    public void Configure(EntityTypeBuilder<Deposit> builder)
    {
        builder.Property(e => e.Amount).HasColumnType("money");
        builder.Property(e => e.RefundedAmount).HasColumnType("money");
    }
}