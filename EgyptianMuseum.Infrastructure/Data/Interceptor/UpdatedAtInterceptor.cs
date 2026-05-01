using EgyptianMuseum.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgyptianMuseum.Infrastructure.Data.Interceptor
{
    public class UpdatedAtInceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            SetUpdatedAt(eventData);
            return result;
        }
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            SetUpdatedAt(eventData);
            return ValueTask.FromResult(result);
        }
        private static void SetUpdatedAt(DbContextEventData eventData)
        {
            if (eventData.Context == null) return;
            foreach (var entry in eventData.Context.ChangeTracker.Entries())
            {
                if (entry is null || entry.State != EntityState.Modified || !(entry.Entity is BaseEntity entity))
                    continue;

                entity.UpdatedAt = DateTime.UtcNow;

            }
        }
    }
}
