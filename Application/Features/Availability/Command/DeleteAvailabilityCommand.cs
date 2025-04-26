using FluentResults;
using MediatR;

namespace Application.Features.Availability.Command
{
    public class DeleteAvailabilityCommand:IRequest<Result<int>>
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }

    }
}
