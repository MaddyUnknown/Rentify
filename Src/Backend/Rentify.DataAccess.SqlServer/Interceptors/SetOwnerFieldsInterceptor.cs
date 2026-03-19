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
            foreach (var entity in context.ChangeTracker.Entries<ISubscriptionEntity>())
            {
                if (entity.State != EntityState.Added) continue;

                var ownerId = entity.Property(e => e.SubscriptionId).CurrentValue;
                var ownerEntity = entity.Property(e => e.Subscription).CurrentValue;

                if (ownerId != 0 || ownerEntity != null) continue;

                entity.Property(e => e.SubscriptionId).CurrentValue = 1;
            }
        }
    }
}
