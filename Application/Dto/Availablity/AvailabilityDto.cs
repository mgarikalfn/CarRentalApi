using CarRentalApi.Entities;

namespace CarRentalApi.Dto.Availablity
{
    public class AvailabilityDto
    {
        public int VehicleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public AvailabilityStatus Status { get; set; }
    }
}
