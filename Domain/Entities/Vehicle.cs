using Domain.Common;

namespace Domain.Entities;

public class Vehicle : AggregateRoot
{
    public Guid OwnerId { get; private set; }

    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    public VehicleSpecification Specification { get; private set; } = null!;
    public Money Price { get; private set; } = null!;
    public GeoLocation? Location { get; private set; }

    public int Mileage { get; private set; }
    public VehicleStatus Status { get; private set; }

    public decimal AverageRating { get; private set; }
    public int ReviewCount { get; private set; }

    private readonly List<VehiclePhoto> _photos = [];
    public IReadOnlyCollection<VehiclePhoto> Photos => _photos.AsReadOnly();

    private const int MaxPhotos = 10;

    private Vehicle() { } // EF Core

    private Vehicle(
        Guid ownerId,
        string title,
        string description,
        VehicleSpecification specification,
        Money price,
        int mileage)
    {
        OwnerId = ownerId;
        Title = ValidateTitle(title);
        Description = description?.Trim() ?? string.Empty;
        Specification = specification;
        Price = price;
        Mileage = ValidateMileage(mileage);
        Status = VehicleStatus.PendingApproval;

        AddDomainEvent(new VehicleCreatedEvent(Id, OwnerId));
    }

    public static Vehicle Create(
        Guid ownerId,
        string title,
        string description,
        VehicleSpecification specification,
        Money price,
        int mileage)
    {
        if (ownerId == Guid.Empty)
            throw new DomainException("OwnerId is required.");

        ArgumentNullException.ThrowIfNull(specification);
        ArgumentNullException.ThrowIfNull(price);

        return new Vehicle(ownerId, title, description, specification, price, mileage);
    }

    public void PublishListing()
    {
        if (Location is null)
            throw new DomainException("Vehicle cannot be published without a location.");

        if (_photos.Count == 0)
            throw new DomainException("Vehicle cannot be published without at least one photo.");

        if (Status == VehicleStatus.Suspended)
            throw new DomainException("Suspended vehicles cannot be published directly; contact support.");

        Status = VehicleStatus.Available;
        AddDomainEvent(new VehicleListedEvent(Id, OwnerId));
    }

    public void Unlist()
    {
        Status = VehicleStatus.Inactive;
        AddDomainEvent(new VehicleUnlistedEvent(Id));
    }

    public void MarkUnderMaintenance()
    {
        if (Status == VehicleStatus.PendingApproval)
            throw new DomainException("Vehicle must be approved before it can be marked under maintenance.");

        Status = VehicleStatus.Maintenance;
    }

    public void ReturnFromMaintenance()
    {
        if (Status != VehicleStatus.Maintenance)
            throw new DomainException("Vehicle is not currently under maintenance.");

        Status = VehicleStatus.Available;
    }

    public void Suspend(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("A reason is required to suspend a listing.");

        Status = VehicleStatus.Suspended;
        AddDomainEvent(new VehicleSuspendedEvent(Id, reason));
    }

    public void UpdatePrice(Money newPrice)
    {
        ArgumentNullException.ThrowIfNull(newPrice);
        Price = newPrice;
    }

    public void UpdateLocation(GeoLocation newLocation)
    {
        ArgumentNullException.ThrowIfNull(newLocation);
        Location = newLocation;
    }

    public void UpdateMileage(int newMileage)
    {
        var validated = ValidateMileage(newMileage);

        if (validated < Mileage)
            throw new DomainException("Mileage cannot decrease.");

        Mileage = validated;
    }

    public void AddPhoto(string url, int displayOrder)
    {
        if (_photos.Count >= MaxPhotos)
            throw new DomainException($"Only {MaxPhotos} photos are allowed per vehicle.");

        _photos.Add(new VehiclePhoto(Id, url, displayOrder));
    }

    public void RemovePhoto(Guid photoId)
    {
        var photo = _photos.FirstOrDefault(p => p.Id == photoId)
            ?? throw new DomainException("Photo not found.");

        _photos.Remove(photo);
    }

    public void RecalculateAverageRating(decimal newAverage, int reviewCount)
    {
        if (newAverage < 0 || newAverage > 5)
            throw new DomainException("Average rating must be between 0 and 5.");

        AverageRating = newAverage;
        ReviewCount = reviewCount;
    }

    private static string ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Title is required.");

        title = title.Trim();

        if (title.Length > 100)
            throw new DomainException("Title cannot exceed 100 characters.");

        return title;
    }

    private static int ValidateMileage(int mileage)
    {
        if (mileage < 0)
            throw new DomainException("Mileage cannot be negative.");

        return mileage;
    }
}

public enum VehicleStatus
{
    PendingApproval,
    Available,
    Maintenance,
    Inactive,
    Suspended
    // Deliberately no "Booked" status — whether a vehicle is currently
    // booked is derived from Booking/VehicleAvailability records, never
    // stored statically on Vehicle. See ddd-conventions.md.
}