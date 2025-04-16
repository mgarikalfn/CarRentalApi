using System.ComponentModel.DataAnnotations;
using CarRentalApi.Entities;

namespace CarRentalApi.Dto.vehicle
{
    public class CreateVehicleDto
    {
        [Required]
        public string Make { get; set; }

        [Required]
        public string Model { get; set; }

        [Required]
        [Range(1900, 2100)]
        public int Year { get; set; }

        [Required]
        public string LicensePlate { get; set; }

        [Required]
        public string Color { get; set; }

        [Range(0, int.MaxValue)]
        public int Mileage { get; set; }

        [Required]
        public string TransmissionType { get; set; }

        [Required]
        public string FuelType { get; set; }

        [Range(1, 20)]
        public int Seats { get; set; }

        public string Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal DailyPrice { get; set; }

        public List<IFormFile> Images { get; set; } = new List<IFormFile>();

    }
}



