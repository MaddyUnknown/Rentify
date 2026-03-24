using Microsoft.EntityFrameworkCore;
using Rentify.Core.Abstractions.Accessors;
using Rentify.Core.Abstractions.Entities;
using Rentify.Core.Enums;
using Rentify.DataAccess.SqlServer.Constants;
using Rentify.DataAccess.SqlServer.Data;
using Rentify.DataAccess.SqlServer.Interfaces.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.DataAccess.SqlServer.Filters
{
    public class GlobalSubscriptionFilter : IGlobalFilter
    {
        public void OnModelCreating(ModelBuilder modelBuilder, RentifyDbContext context)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ISubscriptionEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var method = typeof(GlobalSubscriptionFilter)
                        .GetMethod(nameof(SetTenantFilter), BindingFlags.NonPublic | BindingFlags.Instance)?
                        .MakeGenericMethod(entityType.ClrType);

                    if (method == null) throw new InvalidOperationException(string.Format(DataAccessConstant.MethodNotFound, nameof(GlobalSubscriptionFilter), nameof(SetTenantFilter)));

                    method.Invoke(this, new object[] { modelBuilder, context });
                }
            }
        }

        private void SetTenantFilter<TEntity>(ModelBuilder modelBuilder, RentifyDbContext context) where TEntity : class, ISubscriptionEntity
        {
            modelBuilder.Entity<TEntity>()
                .HasQueryFilter(e => context.DataScope.DataScopeMode == DataScopeModeEnum.FullAccess || e.SubscriptionId == context.DataScope.SubscriptionId);
        }
    }
}
