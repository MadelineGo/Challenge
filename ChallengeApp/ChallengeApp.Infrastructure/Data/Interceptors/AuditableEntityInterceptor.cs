using ChallengeApp.Domain.Primitives;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ChallengeApp.Infrastructure.Data.Interceptors;

public class AuditableEntityInterceptor: SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            foreach (var entry in eventData.Context.ChangeTracker.Entries<IAuditableEntity>())
            {
                if (entry.State is EntityState.Added or EntityState.Modified ||
                    entry.HasChangedOwnedEntities())
                {

                    var now = DateTimeOffset.UtcNow;
                    if(entry.State == EntityState.Added)
                        entry.Entity.CreatedDate = now;
                        
                    entry.Entity.LastModifiedDate = now;

                }
            }
        }
        
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}

public static class EntryExtensions
{
    public static bool HasChangedOwnedEntities(this EntityEntry entry)
    {
        return entry.References.Any(r => r.TargetEntry != null
                                         && r.TargetEntry.Metadata.IsOwned()
                                         && r.TargetEntry.State is EntityState.Added or EntityState.Modified);
    }
}
