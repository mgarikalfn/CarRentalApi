using Domain.Common;

namespace Domain.Entities;

public record BookingCreatedEvent(Guid BookingId, Guid VehicleId, Guid RenterId) : IDomainEvent;

public record BookingApprovedEvent(Guid BookingId, Guid VehicleId, Guid RenterId) : IDomainEvent;

public record BookingRejectedEvent(Guid BookingId, string Reason) : IDomainEvent;

public record BookingCancelledEvent(Guid BookingId, string Reason) : IDomainEvent;

public record BookingActivatedEvent(Guid BookingId, Guid VehicleId) : IDomainEvent;

public record BookingCompletedEvent(Guid BookingId, Guid VehicleId, Guid RenterId) : IDomainEvent;