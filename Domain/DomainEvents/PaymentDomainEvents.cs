using Domain.Common;
using Domain.Entities;

namespace Domain.Entities;

public record PaymentCreatedEvent(Guid PaymentId, Guid BookingId, Guid PayerId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record PaymentSucceededEvent(Guid PaymentId, Guid BookingId, Money PaidAmount) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record PaymentFailedEvent(Guid PaymentId, Guid BookingId, string Reason) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record PaymentRefundedEvent(Guid PaymentId, Guid BookingId, Money PaidAmount) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
