using Domain.Common;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    private readonly List<Vehicle> _ownedVehicles = [];
    private readonly List<Booking> _bookings = [];
    private readonly List<Payment> _payments = [];
    private readonly List<Review> _givenReviews = [];
    private readonly List<Review> _receivedReviews = [];
    private readonly List<VerificationRecord> _verificationRecords = [];

    private ApplicationUser()
    {
        // Required by EF Core
    }

    private ApplicationUser(
        string firstName,
        string lastName,
        string email)
    {
        UpdateName(firstName, lastName);

        Email = email;
        UserName = email;

        Status = UserStatus.Active;
        CreatedAt = DateTime.UtcNow;
    }

    // ==========================
    // Identity Information
    // ==========================

    public string FirstName { get; private set; } = string.Empty;

    public string LastName { get; private set; } = string.Empty;

    public UserStatus Status { get; private set; }

    public string? ProfilePhotoUrl { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? LastModifiedAt { get; private set; }

    public string? DriverLicenseNumber { get; private set; }

    public bool IsDriverLicenseVerified { get; private set; }

    public string FullName => $"{FirstName} {LastName}";

    // ==========================
    // Navigation Properties
    // ==========================

    public TrustProfile TrustProfile { get; private set; } = null!;

    public IReadOnlyCollection<Vehicle> OwnedVehicles => _ownedVehicles;

    public IReadOnlyCollection<Booking> Bookings => _bookings;

    public IReadOnlyCollection<Payment> Payments => _payments;

    public IReadOnlyCollection<Review> GivenReviews => _givenReviews;

    public IReadOnlyCollection<Review> ReceivedReviews => _receivedReviews;

    public IReadOnlyCollection<VerificationRecord> VerificationRecords
        => _verificationRecords;

    // ==========================
    // Factory
    // ==========================

    public static ApplicationUser Register(
        string firstName,
        string lastName,
        string email)
    {
        return new ApplicationUser(
            firstName,
            lastName,
            email);
    }

    // ==========================
    // Domain Behavior
    // ==========================

    public void UpdateName(
        string firstName,
        string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("First name is required.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("Last name is required.");

        FirstName = firstName.Trim();
        LastName = lastName.Trim();

        MarkModified();
    }

    public void UpdateProfilePhoto(string photoUrl)
    {
        if (string.IsNullOrWhiteSpace(photoUrl))
            throw new DomainException("Profile photo URL cannot be empty.");

        ProfilePhotoUrl = photoUrl;

        MarkModified();
    }

    public void Suspend()
    {
        if (Status == UserStatus.Suspended)
            throw new DomainException("User is already suspended.");

        Status = UserStatus.Suspended;

        MarkModified();
    }

    public void Activate()
    {
        if (Status == UserStatus.Active)
            throw new DomainException("User is already active.");

        Status = UserStatus.Active;

        MarkModified();
    }

    public void AddDriverLicense(string licenseNumber, DateTime expiryDate)
    {
        if (string.IsNullOrWhiteSpace(licenseNumber))
            throw new DomainException("License number is required.");
        if (expiryDate <= DateTime.UtcNow.Date)
            throw new DomainException("Driver license has expired.");
        DriverLicenseNumber = licenseNumber.Trim();
        IsDriverLicenseVerified = false;
        MarkModified();
    }

    private void MarkModified()
    {
        LastModifiedAt = DateTime.UtcNow;
    }
}