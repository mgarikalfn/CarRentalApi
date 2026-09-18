

using Application.Dto.vehicle;
using FluentResults;
using MediatR;

namespace Application.Features.Vehicles
{
    public class GetVehicleByIdQuery:IRequest<Result<VehicleDto>>
    {
        public Guid Id { get; set; }
    }
}
