using MediatR;

namespace CarRentalApi.Application.Availability.Command
{
    public class DeleteAvailabilityCommand:IRequest<int>
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }

    }
}
