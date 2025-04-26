
using FluentResults;
using MediatR;

namespace Application.Features.Vehicle.Command
{
    public class DeleteVehicleCommand:IRequest<Result<int>>
    {
        public string OwnerId { get; set; }
        public int Id { get; set; }
    }
}
