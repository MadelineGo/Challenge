using ChallengeApp.Domain.Enums;

namespace ChallengeApp.Domain.Primitives;

public class EventMessage(Operation operation)
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime CreationDate { get; } = DateTime.UtcNow;
    public string Operation { get; } = operation.ToString();
}