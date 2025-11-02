namespace ChallengeApp.Domain.Primitives;

public interface IEventBus : IDisposable
{
    Task Publish<T>(T @event) where T : EventMessage;
}