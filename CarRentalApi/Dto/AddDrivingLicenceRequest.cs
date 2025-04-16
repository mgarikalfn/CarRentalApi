using System.ComponentModel.DataAnnotations;

namespace CarRentalApi.Dto
{
    public class AddDrivingLicenceRequest
    {
        [Required]
        [StringLength(20, MinimumLength = 5)]
        public string LicenseNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime ExpiryDate { get; set; }
    }
}
