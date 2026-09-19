using Domain.Common;

namespace Domain.Entities;

public record DamageReportCreatedEvent(
    Guid DamageReportId,
    Guid BookingId,
    Guid VehicleId,
    Guid ReportedByUserId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Raised only on ResolvedAtFault and ResolvedNotAtFault — NOT on Dismissed.
/// AtFault=true means renter is at fault; false means not at fault.
/// Future handler will recalculate TrustProfile (deferred — same pattern as
/// ReviewSubmittedEvent which is also raised but not yet wired to a handler).
/// </summary>
public record DamageReportResolvedEvent(
    Guid DamageReportId,
    Guid BookingId,
    Guid VehicleId,
    bool AtFault) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
