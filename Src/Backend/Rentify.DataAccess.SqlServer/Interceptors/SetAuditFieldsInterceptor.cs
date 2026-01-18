using Microsoft.EntityFrameworkCore;
using Rentify.Core.Abstractions.Entities;
using Rentify.DataAccess.SqlServer.Interfaces.Interceptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.DataAccess.SqlServer.Interceptors
{
    public class SetAuditFieldsInterceptor : ISaveChangesInterceptor
    {
        public void OnSaveChange(DbContext context)
        {
            foreach (var entry in context.ChangeTracker.Entries())
            {
                // Owned entity would use there parents audit fields
                if (entry.Metadata.IsOwned() && (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.State == EntityState.Deleted))
                {
                    var ownership = entry.Metadata.FindOwnership();
                    if (ownership == null) continue;

                    var ownerPrimaryKeyValue = ownership.Properties.Select(p => entry.Property(p.Name).CurrentValue).ToArray();
                    var ownerEntity = context.Find(ownership.PrincipalEntityType.ClrType, ownerPrimaryKeyValue);
                    if (ownerEntity == null) continue;

                    var ownerEntry = context.Entry(ownerEntity);

                    if (ownerEntry.State == EntityState.Unchanged && ownerEntry.Entity is EntityBase)
                    {
                        ownerEntry.Property(nameof(EntityBase.ModifiedDate)).CurrentValue = DateTime.Now;
                    }
                }
                // Standalone entity would contain there own set of audit fields
                else if (!entry.Metadata.IsOwned() && entry.Entity is EntityBase)
                {

                    if (entry.State == EntityState.Added)
                    {
                        entry.Property(nameof(EntityBase.CreatedDate)).CurrentValue = DateTime.Now;
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        entry.Property(nameof(EntityBase.ModifiedDate)).CurrentValue = DateTime.Now;
                    }
                }
            }
        }
    }
}
