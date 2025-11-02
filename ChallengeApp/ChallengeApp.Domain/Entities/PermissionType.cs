using ChallengeApp.Domain.Primitives;

namespace ChallengeApp.Domain.Entities;

public class PermissionType : BaseEntity
{
    public int Id { get; set; }
    public required string Description { get; set; }
    
    public ICollection<Permission> Permissions { get; set; }
}