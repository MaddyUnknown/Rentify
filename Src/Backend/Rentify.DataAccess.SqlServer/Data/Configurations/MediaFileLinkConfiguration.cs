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
    public class MediaFileLinkConfiguration : IEntityTypeConfiguration<MediaFileLink>
    {
        public void Configure(EntityTypeBuilder<MediaFileLink> builder)
        {
            builder.HasMany(l => l.Tags)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
