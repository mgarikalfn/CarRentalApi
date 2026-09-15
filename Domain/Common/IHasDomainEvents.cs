namespace Domain.Common;

/// <summary>
/// Interface for domain entities that can raise domain events.
/// Implemented by AggregateRoot and other root entities (like ApplicationUser)
/// that need to track and dispatch domain events but don't inherit from AggregateRoot
/// (e.g., entities that inherit from ASP.NET Core Identity types).
/// 
/// AppDbContext's domain event dispatcher queries ChangeTracker.Entries<IHasDomainEvents>()
/// to catch all entities with pending domain events, regardless of whether they inherit
/// from AggregateRoot or implement this interface directly.
/// </summary>
public interface IHasDomainEvents
{
    /// <summary>
    /// Collection of domain events that have been raised by this entity but not yet dispatched.
    /// </summary>
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    /// <summary>
    /// Clears all domain events from this entity. Called by infrastructure after events are dispatched.
    /// </summary>
    void ClearDomainEvents();
}
