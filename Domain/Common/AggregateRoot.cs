namespace Domain.Common;

public abstract class AggregateRoot : AuditableEntity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    // IReadOnlyCollection so nothing outside the aggregate can Add/Remove
    // directly — only the aggregate itself decides when an event happens.
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    // Called by infrastructure (EF Core SaveChanges interceptor, typically)
    // AFTER the events have been dispatched, so the same event doesn't fire
    // twice if this aggregate is saved again later.
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}