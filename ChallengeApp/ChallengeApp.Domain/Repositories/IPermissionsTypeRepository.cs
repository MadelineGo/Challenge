using ChallengeApp.Domain.Entities;

namespace ChallengeApp.Domain.Repositories;

public interface IPermissionsTypeRepository
{
    Task<IEnumerable<PermissionType>> GetAllAsync(CancellationToken cancellationToken);
}