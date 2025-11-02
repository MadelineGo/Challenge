using ChallengeApp.Domain.Enums;
using ChallengeApp.Domain.Events;
using ChallengeApp.Domain.Primitives;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ChallengeApp.Application.EventHandlers;

public class PermissionsGetEventHandler(ILogger<PermissionsGetEventHandler>  logger, IEventBus bus) : INotificationHandler<GetPermissionsEvent>
{
    private readonly ILogger<PermissionsGetEventHandler> _logger = logger;
    private readonly IEventBus _eventBus = bus;
    public async Task Handle(GetPermissionsEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Event {DomainEvent} published at {Time}", notification.GetType().Name, DateTime.UtcNow);
        await _eventBus.Publish(new EventMessage(Operation.Get));
    }
}