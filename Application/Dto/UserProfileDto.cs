namespace CarRentalApi.Dto
{
    public class UserProfileDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string DriverLicenseNumber { get; set; }
        public DateTime? DriverLicenseExpiryDate { get; set; }
        public bool IsDriverLicenseVerified { get; set; }
    }
}
