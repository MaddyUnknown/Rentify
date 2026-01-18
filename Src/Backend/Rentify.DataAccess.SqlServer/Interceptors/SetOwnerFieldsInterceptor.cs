using Microsoft.EntityFrameworkCore;
using Rentify.Core.Abstractions.Entities;
using Rentify.DataAccess.SqlServer.Data;
using Rentify.DataAccess.SqlServer.Interfaces.Interceptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.DataAccess.SqlServer.Interceptors
{
    public class SetOwnerFieldsInterceptor : ISaveChangesInterceptor
    {
        public void OnSaveChange(DbContext context)
        {
            // TO-DO: To be changed to use dynamic value
            foreach (var entity in context.ChangeTracker.Entries<IOwnedEntity>())
            {
                if (entity.State == EntityState.Added)
                {
                    entity.Property(e => e.OwnerId).CurrentValue = 1;
                }
            }
        }
    }
}
