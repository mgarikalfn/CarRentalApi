using CarRentalApi.Entities;
using MediatR;

namespace CarRentalApi.Application.Availability.Command
{
    public class CreateAvailabilityCommand:IRequest<CreateAvailabilityResponseDto>
    {
        public string OwnerId { get; set; } 
        public int VehicleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public AvailabilityStatus Status { get; set; } // Available, Blocked, Booked
    }
    
}
