using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Vehicle:BaseEntity
    {
      
        public string OwnerId { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string LicensePlate { get; set; }
        public string Color { get; set; }
        public int Mileage { get; set; }
        public string TransmissionType { get; set; }
        public string FuelType { get; set; }
        public int Seats { get; set; }
        public string Description { get; set; }
        public decimal DailyPrice { get; set; }
        public bool IsAvailable { get; set; } = true;
        

        // Navigation properties
        public ApplicationUser Owner { get; set; }
        public ICollection<VehicleImage> Images { get; set; }
        public ICollection<Availability> Availabilities { get; set; }
        public ICollection<Booking> Bookings { get; set; }
        public ICollection<DamageReport> DamageReports { get; set; }
        public ICollection<VehicleFeatures> VehicleFeatures { get; set; }
    }
}
