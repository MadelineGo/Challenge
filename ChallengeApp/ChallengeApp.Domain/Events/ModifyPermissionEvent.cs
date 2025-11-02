using ChallengeApp.Domain.Entities;

namespace ChallengeApp.Domain.Events;

public class ModifyPermissionEvent(Permission permission) : BaseEvent
{
    public Permission Permission { get; } =  permission;
}