using Application.Common;
using MediatR;

namespace Application.Features.Availabilities.Command
{
    public class DeleteAvailabilityCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public Guid VehicleId { get; set; }
    }
}
