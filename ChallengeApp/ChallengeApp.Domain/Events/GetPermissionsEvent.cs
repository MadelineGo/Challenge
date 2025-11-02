using ChallengeApp.Domain.Entities;
using MediatR;

namespace ChallengeApp.Domain.Events;

public class GetPermissionsEvent(IEnumerable<Permission> permissions) : BaseEvent
{
    public IEnumerable<Permission> Permissions { get; } = permissions;
}