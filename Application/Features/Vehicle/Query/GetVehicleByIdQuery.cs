

using Application.Dto.vehicle;
using FluentResults;
using MediatR;

namespace Application.Features.Vehicle
{
    public class GetVehicleByIdQuery:IRequest<Result<VehicleDto>>
    {
        public int Id { get; set; }
    }
}
