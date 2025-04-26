using System.ComponentModel.DataAnnotations;

namespace CarRentalApi.Dto
{
    public class RegisterDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string? DriverLicenseNumber { get; set; }
        public DateTime? DriverLicenseExpiryDate { get; set; }

        public string? ChooseRole { get; set; } = "Renter"; // Default role is "User"
    }
}
