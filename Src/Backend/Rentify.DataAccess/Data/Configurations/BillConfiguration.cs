using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rentify.Core.Entities;

namespace Rentify.DataAccess.Data.Configurations;

public class BillConfiguration : IEntityTypeConfiguration<Bill>
{
    public void Configure(EntityTypeBuilder<Bill> builder)
    {
        builder.Property(e => e.AmountPayable).HasColumnType("money");
        builder.Property(e => e.AmountPaid).HasColumnType("money");
        builder.HasIndex(e => new { e.BillNumber, e.BillVersion }).IsUnique();
        builder.HasMany(b => b.BillLineItems)
            .WithOne()
            .HasForeignKey(bli => bli.BillId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(b => b.Payments)
            .WithOne()
            .HasForeignKey(p => p.BillId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}