using Domain.Common;

namespace Domain.Entities;

public record ReviewSubmittedEvent(
    Guid ReviewId,
    Guid BookingId,
    Guid RevieweeId,
    Guid VehicleId,
    int Rating) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
