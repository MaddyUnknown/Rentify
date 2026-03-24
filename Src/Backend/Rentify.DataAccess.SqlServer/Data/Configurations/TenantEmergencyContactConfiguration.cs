using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rentify.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.DataAccess.SqlServer.Data.Configurations
{
    public class TenantEmergencyContactConfiguration : IEntityTypeConfiguration<TenantEmergencyContact>
    {
        public void Configure(EntityTypeBuilder<TenantEmergencyContact> builder)
        {
            builder.HasOne(p => p.Subscription)
                .WithMany()
                .HasForeignKey(p => p.SubscriptionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
