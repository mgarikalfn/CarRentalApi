using MediatR;

namespace CarRentalApi.Application.Vehicle.command
{
    public class DeleteVehicleCommand:IRequest<int>
    {
        public string OwnerId { get; set; }
        public int Id { get; set; }
    }
}
