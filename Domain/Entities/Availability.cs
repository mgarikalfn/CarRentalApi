using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class Availability : Entity
{
    public Guid VehicleId { get; private set; }

    public DateTime StartDate { get; private set; }

    public DateTime EndDate { get; private set; }

    public AvailabilityType Type { get; private set; }

    public AvailabilityStatus Status { get; private set; }

    private Availability(){}

    private Availability(
        Guid vehicleId,
        DateTime start,
        DateTime end,
        AvailabilityType type)
    {
        if(start >= end)
            throw new DomainException(
                "Start date must be before end date.");

        VehicleId = vehicleId;

        StartDate = start;

        EndDate = end;

        Type = type;

        Status = AvailabilityStatus.Active;
    }

    public static Availability Create(
        Guid vehicleId,
        DateTime start,
        DateTime end,
        AvailabilityType type)
    {
        return new Availability(
            vehicleId,
            start,
            end,
            type);
    }

    public void Cancel()
    {
        Status = AvailabilityStatus.Cancelled;
    }
}