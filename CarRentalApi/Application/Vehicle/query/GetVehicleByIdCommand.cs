using CarRentalApi.Dto.vehicle;
using MediatR;

namespace CarRentalApi.Application.Vehicle.query
{
    public class GetVehicleByIdCommand:IRequest<VehicleDto>
    {
        public int Id { get; set; }
    }
}
