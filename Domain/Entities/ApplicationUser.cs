using Domain.Enums;
using Microsoft.AspNetCore.Identity;
namespace Domain.Entities;

    public class ApplicationUser : IdentityUser
{
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;

    public UserRole Role {get; private set;}= UserRole.Renter;

    public UserStatus Status {get; private set;} = UserStatus.Active;

    public string? ProfilePhotoUrl{get; private set;}

    public DateTime CreatedAt { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }

    public ICollection<VerificationRecord> verificationRecords{get; private set;} = new List<VerificationRecord>();
    public ICollection<Vehicle> OwnedVehicles { get; private set; }
        = new List<Vehicle>();

    public ICollection<Booking> Bookings { get; private set; }
        = new List<Booking>();

    public ICollection<Payment> Payments { get; private set; }
        = new List<Payment>();

    public ICollection<Review> GivenReviews { get; private set; }
        = new List<Review>();

    public ICollection<Review> ReceivedReviews { get; private set; }
        = new List<Review>();
}
    