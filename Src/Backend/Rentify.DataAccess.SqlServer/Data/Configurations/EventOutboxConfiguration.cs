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
    public class EventOutboxConfiguration : IEntityTypeConfiguration<EventOutbox>
    {
        public void Configure(EntityTypeBuilder<EventOutbox> builder)
        {
            builder.ToTable(nameof(RentifyDbContext.EventOutboxEntries), "event");
        }
    }
}
