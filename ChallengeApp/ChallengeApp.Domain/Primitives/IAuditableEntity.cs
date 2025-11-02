namespace ChallengeApp.Domain.Primitives;

public interface IAuditableEntity
{
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset LastModifiedDate { get; set; }
}