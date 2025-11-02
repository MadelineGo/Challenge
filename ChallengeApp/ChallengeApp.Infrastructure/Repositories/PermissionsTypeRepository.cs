using ChallengeApp.Domain.Entities;
using ChallengeApp.Domain.Repositories;
using ChallengeApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ChallengeApp.Infrastructure.Repositories;

public class PermissionsTypeRepository(AppDbContext context) : IPermissionsTypeRepository
{
    private readonly DbSet<PermissionType> _permissionType = context.Set<PermissionType>();
    public async Task<IEnumerable<PermissionType>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _permissionType
            .ToListAsync(cancellationToken);
    }
}