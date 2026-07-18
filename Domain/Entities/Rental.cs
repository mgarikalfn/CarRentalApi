using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// Owned type — no Id, no independent identity. Only ever exists embedded
/// inside a Booking (EF Core: builder.OwnsOne(b => b.Rental)). Represents
/// the ACTUAL pickup/return facts, distinct from Booking's PLANNED
/// StartDate/EndDate — these can legitimately differ (late pickup, early
/// or late return).
/// </summary>
public class Rental
{
    public DateTime? ActualPickUpTime { get; private set; }
    public DateTime? ActualReturnTime { get; private set; }
    public int? PickUpMileage { get; private set; }
    public int? ReturnMileage { get; private set; }

    // Parameterless constructor for EF Core materialization.
    // An "empty" Rental means neither pickup nor return has happened yet —
    // this is what makes "car has been picked up but not yet returned"
    // representable, which a single all-at-once constructor cannot do.
    public Rental() { }

    // Public, not private — the whole point is that a handler calls this
    // when the renter actually arrives to pick up the car, independently
    // and at a different time than the return.
    public void RecordPickup(DateTime pickUpTime, int pickUpMileage)
    {
        if (ActualPickUpTime is not null)
            throw new DomainException("Pickup has already been recorded for this rental.");

        if (pickUpMileage < 0)
            throw new DomainException("Pickup mileage cannot be negative.");

        ActualPickUpTime = pickUpTime;
        PickUpMileage = pickUpMileage;
    }

    public void RecordReturn(DateTime returnTime, int returnMileage)
    {
        if (ActualPickUpTime is null || PickUpMileage is null)
            throw new DomainException("Cannot record a return before pickup has been recorded.");

        if (ActualReturnTime is not null)
            throw new DomainException("Return has already been recorded for this rental.");

        if (returnTime <= ActualPickUpTime)
            throw new DomainException("Return time must be after pickup time.");

        if (returnMileage < PickUpMileage)
            throw new DomainException("Return mileage cannot be less than pickup mileage.");

        ActualReturnTime = returnTime;
        ReturnMileage = returnMileage;
    }

    public int? DistanceDriven =>
        (PickUpMileage is not null && ReturnMileage is not null)
            ? ReturnMileage - PickUpMileage
            : null;

    public bool IsPickedUp => ActualPickUpTime is not null;
    public bool IsReturned => ActualReturnTime is not null;
}