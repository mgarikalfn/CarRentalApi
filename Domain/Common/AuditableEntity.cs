using Domain.Common;

public abstract class AuditableEntity : Entity
{
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;

    public DateTime? LastModifiedAt { get; protected set; }

    protected void MarkModified()
    {
        LastModifiedAt = DateTime.UtcNow;
    }
}
