namespace Domain.Common;

/// <summary>
/// Base class for aggregate roots. Provides domain event tracking and dispatch.
/// Implements IHasDomainEvents so AppDbContext can discover and dispatch events.
/// </summary>
public abstract class AggregateRoot : AuditableEntity, IHasDomainEvents
{
    private readonly List<IDomainEvent> _domainEvents = [];

    // IReadOnlyCollection so nothing outside the aggregate can Add/Remove
    // directly — only the aggregate itself decides when an event happens.
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    // Called by infrastructure (AppDbContext.SaveChangesAsync override)
    // AFTER the events have been dispatched, so the same event doesn't fire
    // twice if this aggregate is saved again later.
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}