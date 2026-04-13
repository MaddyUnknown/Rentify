using Microsoft.EntityFrameworkCore;
using Rentify.Core.Abstractions.Accessors;
using Rentify.Core.Abstractions.Entities;
using Rentify.Core.Enums;
using Rentify.DataAccess.SqlServer.Constants;
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
        private IDataScopeAccessor _subscriptionContextAccessor;

        public SetOwnerFieldsInterceptor(IDataScopeAccessor subscriptionContextAccessor)
        {
            _subscriptionContextAccessor = subscriptionContextAccessor;
        }

        public void OnSaveChange(DbContext context)
        {
            // In scoped to full access then entity is not part of any subscription
            if (_subscriptionContextAccessor.DataScope.DataScopeMode == DataScopeModeEnum.FullAccess) return;

            foreach (var entity in context.ChangeTracker.Entries<ISubscriptionEntity>())
            {
                if (entity.State != EntityState.Added) continue;

                var ownerId = entity.Property(e => e.SubscriptionId).CurrentValue;

                if (ownerId != 0) continue;

                entity.Property(e => e.SubscriptionId).CurrentValue = _subscriptionContextAccessor.DataScope.SubscriptionId ?? 0;
            }
        }
    }
}
