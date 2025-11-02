using ChallengeApp.Domain.Entities;

namespace ChallengeApp.Domain.Events;

public class RequestPermissionEvent(Permission permission) : BaseEvent
{
    public Permission Permission { get; } = permission;
}