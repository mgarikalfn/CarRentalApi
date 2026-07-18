using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class Booking : AggregateRoot
{
    public Guid VehicleId { get; private set; }
    public Guid RenterId { get; private set; }

    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    public BookingStatus Status { get; private set; }
    public string PickUpLocation { get; private set; } = string.Empty;
    public string DropOffLocation { get; private set; } = string.Empty;

    public BookingPrice Price { get; private set; } = null!;

    // Owned type — embedded, no independent Id. Null until pickup happens.
    public Rental? Rental { get; private set; }

    public string? CancellationReason { get; private set; }
    public string? RejectionReason { get; private set; }

    private Booking() { } // EF Core

    private Booking(
        Guid vehicleId,
        Guid renterId,
        DateTime startDate,
        DateTime endDate,
        string pickUpLocation,
        string dropOffLocation,
        BookingPrice price)
    {
        if (startDate <= DateTime.UtcNow)
            throw new DomainException("Start date must be in the future.");

        if (endDate <= startDate)
            throw new DomainException("End date must be after start date.");

        if (string.IsNullOrWhiteSpace(pickUpLocation))
            throw new DomainException("Pick-up location is required.");

        if (string.IsNullOrWhiteSpace(dropOffLocation))
            throw new DomainException("Drop-off location is required.");

        VehicleId = vehicleId;
        RenterId = renterId;
        StartDate = startDate;
        EndDate = endDate;
        PickUpLocation = pickUpLocation.Trim();
        DropOffLocation = dropOffLocation.Trim();
        Price = price ?? throw new DomainException("Price is required.");
        Status = BookingStatus.Pending;

        AddDomainEvent(new BookingCreatedEvent(Id, VehicleId, RenterId));
    }

    public static Booking Create(
        Guid vehicleId,
        Guid renterId,
        DateTime startDate,
        DateTime endDate,
        string pickUpLocation,
        string dropOffLocation,
        BookingPrice price)
    {
        if (vehicleId == Guid.Empty)
            throw new DomainException("VehicleId is required.");

        if (renterId == Guid.Empty)
            throw new DomainException("RenterId is required.");

        return new Booking(vehicleId, renterId, startDate, endDate,
            pickUpLocation, dropOffLocation, price);
    }

    // ----- State transitions -----
    // Every guard below checks THIS booking's own current Status
    // (this.Status), never a caller-supplied value. A caller cannot lie
    // about what state the booking is in — that's the entire point.

    public void Approve()
    {
        if (Status != BookingStatus.Pending)
            throw new DomainException("Only a pending booking can be approved.");

        Status = BookingStatus.Approved;
        AddDomainEvent(new BookingApprovedEvent(Id, VehicleId, RenterId));
    }

    public void Reject(string reason)
    {
        if (Status != BookingStatus.Pending)
            throw new DomainException("Only a pending booking can be rejected.");

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("A reason is required to reject a booking.");

        Status = BookingStatus.Rejected;
        RejectionReason = reason.Trim();
        AddDomainEvent(new BookingRejectedEvent(Id, reason));
    }

    public void Cancel(string reason)
    {
        // Correct boolean logic: reject only if status is NEITHER Pending
        // NOR Approved. (Your version used || here, which is always true —
        // see explanation below the code.)
        if (Status != BookingStatus.Pending && Status != BookingStatus.Approved)
            throw new DomainException("Only a pending or approved booking can be cancelled.");

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("A reason is required to cancel a booking.");

        Status = BookingStatus.Cancelled;
        CancellationReason = reason.Trim();
        AddDomainEvent(new BookingCancelledEvent(Id, reason));
    }

    public void Activate(DateTime pickUpTime, int pickUpMileage)
    {
        if (Status != BookingStatus.Approved)
            throw new DomainException("Only an approved booking can be activated.");

        Rental = new Rental();
        Rental.RecordPickup(pickUpTime, pickUpMileage);

        Status = BookingStatus.Active;
        AddDomainEvent(new BookingActivatedEvent(Id, VehicleId));
    }

    public void Complete(DateTime returnTime, int returnMileage)
    {
        if (Status != BookingStatus.Active)
            throw new DomainException("Only an active booking can be completed.");

        if (Rental is null)
            throw new DomainException("Cannot complete a booking with no recorded pickup.");

        Rental.RecordReturn(returnTime, returnMileage);

        Status = BookingStatus.Completed;
        AddDomainEvent(new BookingCompletedEvent(Id, VehicleId, RenterId));
    }
}