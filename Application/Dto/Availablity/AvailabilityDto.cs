using Domain.Enums;

namespace Application.Dto.Availablity
{
    public class AvailabilityDto
    {
        public int VehicleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public AvailabilityStatus Status { get; set; }
    }
}
