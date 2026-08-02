using Domain.Enums;
using Microsoft.AspNetCore.Identity;
namespace Domain.Entities;

    public class ApplicationUser : IdentityUser
{
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private UserRole _role = UserRole.Renter;
    private UserStatus _status = UserStatus.Active;
    private string? _profilePhotoUrl;
    private DateTime _createdAt;
    private DateTime? _lastModifiedAt;
    private ICollection<VerificationRecord> _verificationRecords = new List<VerificationRecord>();
    private ICollection<Vehicle.Vehicle> _ownedVehicles = new List<Vehicle.Vehicle>();
    private ICollection<Booking> _bookings = new List<Booking>();
    private ICollection<Payment> _payments = new List<Payment>();
    private ICollection<Review> _givenReviews = new List<Review>();
    private ICollection<Review> _receivedReviews = new List<Review>();

    public string FirstName
    {
        get => _firstName;
        private set => _firstName = value;
    }

    public string LastName
    {
        get => _lastName;
        private set => _lastName = value;
    }

    public UserRole Role
    {
        get => _role;
        private set => _role = value;
    }

    public UserStatus Status
    {
        get => _status;
        private set => _status = value;
    }

    public string? ProfilePhotoUrl
    {
        get => _profilePhotoUrl;
        private set => _profilePhotoUrl = value;
    }

    public DateTime CreatedAt
    {
        get => _createdAt;
        private set => _createdAt = value;
    }

    public DateTime? LastModifiedAt
    {
        get => _lastModifiedAt;
        private set => _lastModifiedAt = value;
    }

    public ICollection<VerificationRecord> VerificationRecords
    {
        get => _verificationRecords;
        private set => _verificationRecords = value;
    }

    public ICollection<Vehicle.Vehicle> OwnedVehicles
    {
        get => _ownedVehicles;
        private set => _ownedVehicles = value;
    }

    public ICollection<Booking> Bookings
    {
        get => _bookings;
        private set => _bookings = value;
    }

    public ICollection<Payment> Payments
    {
        get => _payments;
        private set => _payments = value;
    }

    public ICollection<Review> GivenReviews
    {
        get => _givenReviews;
        private set => _givenReviews = value;
    }

    public ICollection<Review> ReceivedReviews
    {
        get => _receivedReviews;
        private set => _receivedReviews = value;
    }
}
    