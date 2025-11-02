namespace ChallengeApp.Domain.Repositories;

public interface IUnitOfWork
{
    IPermissionsRepository  PermissionsRepository { get; }
    IPermissionsTypeRepository  PermissionsTypeRepository { get; }
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}