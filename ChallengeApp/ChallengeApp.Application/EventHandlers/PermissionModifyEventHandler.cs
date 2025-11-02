using MediatR;
using Microsoft.Extensions.Logging;
using ChallengeApp.Domain.Enums;
using ChallengeApp.Domain.Events;
using ChallengeApp.Domain.Primitives;
namespace ChallengeApp.Application.EventHandlers;

public class PermissionModifyEventHandler(ILogger<PermissionModifyEventHandler> logger, IEventBus bus) : INotificationHandler<ModifyPermissionEvent>
{
    private readonly ILogger<PermissionModifyEventHandler> _logger = logger;
    private readonly IEventBus _eventBus = bus;
    public async Task Handle(ModifyPermissionEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Event {DomainEvent} published at {Time}", notification.GetType().Name, DateTime.UtcNow);
        await _eventBus.Publish(new EventMessage(Operation.Modify));

    }
}