using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace aidemo.Integrations.Interceptors
{
    public sealed class AuditFieldsSaveChangesInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            if (eventData.Context is not null)
                Populate(eventData.Context);

            return result;
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            if (eventData.Context is not null)
                Populate(eventData.Context);

            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        internal static void Populate(DbContext context)
        {
            var now = DateTimeOffset.Now;
            foreach (var entry in context.ChangeTracker.Entries())
            {
                if (entry.Entity is not object entity) continue;
                var type = entity.GetType();
                var createdDate = type.GetProperty("CreatedDate");
                var lastUpdatedDate = type.GetProperty("LastUpdatedDate");

                // skip entities that don't have the properties
                if (createdDate is null || lastUpdatedDate is null) continue;

                switch (entry.State)
                {
                    case EntityState.Added:
                        createdDate.SetValue(entity, now);
                        lastUpdatedDate.SetValue(entity, now);
                        break;
                    case EntityState.Modified:
                        lastUpdatedDate.SetValue(entity, now);
                        break;

                        // everything else is a no-op
                }
            }
        }
    }
}
