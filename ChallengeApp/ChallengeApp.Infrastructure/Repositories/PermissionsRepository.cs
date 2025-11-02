using ChallengeApp.Domain.Entities;
using ChallengeApp.Domain.Repositories;
using ChallengeApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ChallengeApp.Infrastructure.Repositories;

public class PermissionsRepository(AppDbContext context) : IPermissionsRepository
{
    private readonly DbSet<Permission> _permissions = context.Set<Permission>();
    
    public async Task AddAsync(Permission permission, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(permission);
        await _permissions.AddAsync(permission, cancellationToken);
    }

    public async Task<(IEnumerable<Permission> Permissions, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var query = _permissions.AsQueryable();

        var totalCount = await query.CountAsync(cancellationToken);
        var permissions = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Include(permission => permission.PermissionType)
            .ToListAsync(cancellationToken);

        return (permissions, totalCount);
    }

    public async Task<Permission?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _permissions
            .Include(permission => permission.PermissionType)
            .SingleOrDefaultAsync(permission => permission.Id == id, cancellationToken);
    }

    public void Update(Permission permission)
    {
        ArgumentNullException.ThrowIfNull(permission);
        _permissions.Update(permission);
    }

    public void Remove(Permission permission)
    {
        ArgumentNullException.ThrowIfNull(permission);
        _permissions.Remove(permission);
    }
}
