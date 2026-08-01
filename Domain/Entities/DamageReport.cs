using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class DamageReport : AggregateRoot
{
    public Guid RentalId { get; private set; }

    public Guid VehicleId { get; private set; }

    public Guid ReporterId { get; private set; }


    public string Description { get; private set; } = string.Empty;


    public DamageType DamageType { get; private set; }

    public DamageSeverity Severity { get; private set; }


    public DamageReportStatus Status { get; private set; }


    public bool? IsRenterAtFault { get; private set; }


    public decimal? EstimatedCost { get; private set; }


    private DamageReport()
    {

    }


    private DamageReport(
        Guid rentalId,
        Guid vehicleId,
        Guid reporterId,
        string description,
        DamageType damageType,
        DamageSeverity severity)
    {

        RentalId = rentalId;
        VehicleId = vehicleId;
        ReporterId = reporterId;

        Description = description;

        DamageType = damageType;
        Severity = severity;

        Status = DamageReportStatus.Reported;
    }


    public static DamageReport Create(
        Guid rentalId,
        Guid vehicleId,
        Guid reporterId,
        string description,
        DamageType damageType,
        DamageSeverity severity)
    {
        if(string.IsNullOrWhiteSpace(description))
            throw new DomainException(
                "Damage description is required");


        return new DamageReport(
            rentalId,
            vehicleId,
            reporterId,
            description,
            damageType,
            severity);
    }


    public void Approve(decimal estimatedCost)
    {
        if(Status != DamageReportStatus.UnderReview)
            throw new DomainException(
                "Only reports under review can be approved");


        if(estimatedCost <= 0)
            throw new DomainException(
                "Estimated cost must be positive");


        EstimatedCost = estimatedCost;

        Status = DamageReportStatus.Approved;
    }


    public void AssignFault(bool renterAtFault)
    {
        IsRenterAtFault = renterAtFault;
    }


    public void Resolve()
    {
        if(Status != DamageReportStatus.Approved)
            throw new DomainException(
                "Only approved reports can be resolved");


        Status = DamageReportStatus.Resolved;
    }
}