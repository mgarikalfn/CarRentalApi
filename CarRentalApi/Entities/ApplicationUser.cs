using Microsoft.AspNetCore.Identity;

namespace CarRentalApi.Entities
{
    public class ApplicationUser:IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? DriverLicenseNumber { get; set; }
        public DateTime? DriverLicenseExpiryDate { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDriverLicenseVerified { get; set; }
        public DateTime? DriverLicenseVerifiedDate { get; set; }

        // Navigation properties

        public ICollection<Vehicle> OwnedVehicles { get; set; }
        public ICollection<Booking> Bookings { get; set; }
        public ICollection<Review> GivenReviews { get; set; }
        public ICollection<Review> ReceivedReviews { get; set; }
        public ICollection<Payment> Payments { get; set; }
        public ICollection<DamageReport> ReportedDamages { get; set; }
    }
}
