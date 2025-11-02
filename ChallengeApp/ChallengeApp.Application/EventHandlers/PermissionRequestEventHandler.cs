using MediatR;
using Microsoft.Extensions.Logging;
using ChallengeApp.Domain.Enums;
using ChallengeApp.Domain.Events;
using ChallengeApp.Domain.Primitives;
namespace ChallengeApp.Application.EventHandlers;

public class PermissionRequestEventHandler(ILogger<PermissionRequestEventHandler> logger, IEventBus bus) : INotificationHandler<RequestPermissionEvent>
{
    private readonly ILogger<PermissionRequestEventHandler> _logger = logger;
    private readonly IEventBus _eventBus = bus;
    public async Task Handle(RequestPermissionEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Event {DomainEvent} published at {Time}", notification.GetType().Name, DateTime.UtcNow);
        await _eventBus.Publish(new EventMessage(Operation.Request));
    }
}

