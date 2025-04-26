using CarRentalApi.Entities;

namespace CarRentalApi.Dto.Availablity
{
    public class CreateAvailabilityDto
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public AvailabilityStatus Status { get; set; }
    }
}
