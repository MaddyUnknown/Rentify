using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rentify.Core.Entities;

namespace Rentify.DataAccess.Data.Configurations;

public class BillLineItemConfiguration : IEntityTypeConfiguration<BillLineItem>
{
    public void Configure(EntityTypeBuilder<BillLineItem> builder)
    {
        builder.Property(e => e.LineItemAmount).HasColumnType("money");
        builder.HasOne(bli => bli.UtilityBill)
            .WithMany()
            .HasForeignKey(bli => bli.UtilityBillId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}