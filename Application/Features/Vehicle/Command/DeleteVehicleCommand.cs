using Application.Common;
using MediatR;

namespace Application.Features.Vehicle.Command
{
    public class DeleteVehicleCommand : IRequest<Result<int>>
    {
        public string OwnerId { get; set; }
        public Guid Id { get; set; }
    }
}
