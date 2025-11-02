using ChallengeApp.Domain.Repositories;
using ChallengeApp.Infrastructure.Data;

namespace ChallengeApp.Infrastructure.Repositories;

public class UnitOfWork(AppDbContext  context) : IUnitOfWork
{
    private readonly AppDbContext _context = context;
    private IPermissionsRepository? _permissionsRepository = null;
    private IPermissionsTypeRepository? _permissionsTypeRepository = null;
    
    public IPermissionsRepository PermissionsRepository => _permissionsRepository is null ? _permissionsRepository = new PermissionsRepository(_context) : _permissionsRepository;

    public IPermissionsTypeRepository PermissionsTypeRepository => _permissionsTypeRepository is null ? _permissionsTypeRepository = new PermissionsTypeRepository(_context) : _permissionsTypeRepository;
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}