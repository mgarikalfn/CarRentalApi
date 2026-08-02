using MediatR;

namespace Domain.Common;

/// <summary>
/// Marker interface for domain events. A domain event represents something
/// meaningful that already happened inside an aggregate (past tense naming,
/// e.g. VehicleListedEvent, BookingCompletedEvent). Aggregates raise these
/// without knowing or caring who — if anyone — reacts to them.
/// </summary>
public interface IDomainEvent :INotification
{
    DateTime OccurredOn => DateTime.UtcNow;
}