using ChallengeApp.Domain.Entities;

namespace ChallengeApp.Domain.Repositories;

public interface IPermissionsRepository
{
    Task AddAsync(Permission permission, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Permission> Permissions, int TotalCount)> GetAllAsync(int pageNumber,
                                                                            int pageSize,
                                                                            CancellationToken cancellationToken);
    Task<Permission?> GetByIdAsync(int id, CancellationToken cancellationToken);
    void Update(Permission permission);
    void Remove(Permission permission);
}
