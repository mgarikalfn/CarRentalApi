using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Owned type embedded in DamageReport via OwnsMany.
/// No Id, no independent table.
/// </summary>
public class DamageImage
{
    public string Url { get; private set; } = string.Empty;
    public DateTime UploadedAt { get; private set; }

    // Parameterless constructor for EF Core materialization.
    private DamageImage() { }

    internal DamageImage(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new DomainException("Image URL is required.");
        Url = url.Trim();
        UploadedAt = DateTime.UtcNow;
    }
}

public class DamageReport : AggregateRoot
{
    private const int MaxImages = 10;

    private readonly List<DamageImage> _images = [];

    // ── Identity references (Guid only — no navigation properties) ──────────
    public Guid BookingId { get; private set; }
    public Guid VehicleId { get; private set; }

    /// <summary>FK to ApplicationUser — the renter or host who filed the report.</summary>
    public Guid ReportedByUserId { get; private set; }

    // ── Fields ───────────────────────────────────────────────────────────────
    public string Description { get; private set; } = string.Empty;
    public DamageReportStatus Status { get; private set; }
    public DateTime? ResolvedAt { get; private set; }

    // ── Owned collection ─────────────────────────────────────────────────────
    public IReadOnlyCollection<DamageImage> Images => _images.AsReadOnly();

    // ── EF Core parameterless constructor ────────────────────────────────────
    private DamageReport() { }

    // ── Real constructor ─────────────────────────────────────────────────────
    private DamageReport(
        Guid bookingId,
        Guid vehicleId,
        Guid reportedByUserId,
        string description)
    {
        BookingId        = bookingId;
        VehicleId        = vehicleId;
        ReportedByUserId = reportedByUserId;
        Description      = description;
        Status           = DamageReportStatus.Open;

        AddDomainEvent(new DamageReportCreatedEvent(Id, BookingId, VehicleId, ReportedByUserId));
    }

    // ── Factory ──────────────────────────────────────────────────────────────
    public static DamageReport Create(
        Guid bookingId,
        Guid vehicleId,
        Guid reportedByUserId,
        string description)
    {
        if (bookingId == Guid.Empty)
            throw new DomainException("BookingId is required.");
        if (vehicleId == Guid.Empty)
            throw new DomainException("VehicleId is required.");
        if (reportedByUserId == Guid.Empty)
            throw new DomainException("ReportedByUserId is required.");
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Description is required.");
        if (description.Trim().Length > 2000)
            throw new DomainException("Description cannot exceed 2000 characters.");

        return new DamageReport(bookingId, vehicleId, reportedByUserId, description.Trim());
    }

    // ── Image management ─────────────────────────────────────────────────────
    public void AddImage(string url)
    {
        if (_images.Count >= MaxImages)
            throw new DomainException($"A damage report cannot have more than {MaxImages} images.");
        _images.Add(new DamageImage(url));
    }

    // ── State transitions ────────────────────────────────────────────────────

    /// <summary>Open → UnderReview. Guard: this.Status must be Open.</summary>
    public void StartReview()
    {
        // Guard reads this.Status — never a parameter
        if (Status != DamageReportStatus.Open)
            throw new DomainException("Only an open damage report can be placed under review.");

        Status = DamageReportStatus.UnderReview;
    }

    /// <summary>UnderReview → ResolvedAtFault. Guard: this.Status must be UnderReview.</summary>
    public void ResolveAtFault()
    {
        // Guard reads this.Status — never a parameter
        if (Status != DamageReportStatus.UnderReview)
            throw new DomainException("Only a report under review can be resolved at fault.");

        Status     = DamageReportStatus.ResolvedAtFault;
        ResolvedAt = DateTime.UtcNow;

        AddDomainEvent(new DamageReportResolvedEvent(Id, BookingId, VehicleId, AtFault: true));
    }

    /// <summary>UnderReview → ResolvedNotAtFault. Guard: this.Status must be UnderReview.</summary>
    public void ResolveNotAtFault()
    {
        // Guard reads this.Status — never a parameter
        if (Status != DamageReportStatus.UnderReview)
            throw new DomainException("Only a report under review can be resolved not at fault.");

        Status     = DamageReportStatus.ResolvedNotAtFault;
        ResolvedAt = DateTime.UtcNow;

        AddDomainEvent(new DamageReportResolvedEvent(Id, BookingId, VehicleId, AtFault: false));
    }

    /// <summary>UnderReview → Dismissed. Guard: this.Status must be UnderReview.</summary>
    public void Dismiss()
    {
        // Guard reads this.Status — never a parameter
        if (Status != DamageReportStatus.UnderReview)
            throw new DomainException("Only a report under review can be dismissed.");

        Status = DamageReportStatus.Dismissed;
        // No DamageReportResolvedEvent — Dismissed means no fault determination was made.
    }
}
