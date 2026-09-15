namespace Domain.Common;

/// <summary>
/// Pure marker interface for domain events. Represents something meaningful
/// that already happened inside an aggregate (past tense naming, e.g.,
/// VehicleListedEvent, BookingCompletedEvent). Aggregates raise these
/// without knowing or caring who — if anyone — reacts to them.
/// 
/// IMPORTANT: This interface has ZERO framework dependencies. The Domain
/// layer knows nothing about MediatR, INotification, or any other
/// infrastructure packages. Event dispatch is handled entirely by the
/// Application layer via a DomainEventNotification<TDomainEvent> adapter.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// The exact moment the event occurred. Must be captured at event construction
    /// time (not recomputed on every access). Implementations should set this to
    /// DateTime.UtcNow at construction, or to a specific timestamp if exact timing
    /// matters (e.g., event sourcing, audit trails).
    /// </summary>
    DateTime OccurredOn { get; }
}
