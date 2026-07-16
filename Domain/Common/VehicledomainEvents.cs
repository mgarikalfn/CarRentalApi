using Domain.Common;

namespace Domain.Entities;

public record VehicleCreatedEvent(Guid VehicleId, Guid OwnerId) : IDomainEvent;

public record VehicleListedEvent(Guid VehicleId, Guid OwnerId) : IDomainEvent;

public record VehicleUnlistedEvent(Guid VehicleId) : IDomainEvent;

public record VehicleSuspendedEvent(Guid VehicleId, string Reason) : IDomainEvent;