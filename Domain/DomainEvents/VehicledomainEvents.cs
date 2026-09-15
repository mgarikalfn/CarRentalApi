using Domain.Common;

namespace Domain.Entities;

public record VehicleCreatedEvent(Guid VehicleId, Guid OwnerId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record VehicleListedEvent(Guid VehicleId, Guid OwnerId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record VehicleUnlistedEvent(Guid VehicleId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record VehicleSuspendedEvent(Guid VehicleId, string Reason) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}