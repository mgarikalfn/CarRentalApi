using Application.Common;
using Domain.Enums;
using MediatR;

namespace Application.Features.Availability.Command
{
    public class CreateAvailabilityCommand : IRequest<Result<int>>
    {
        public Guid OwnerId { get; set; }
        public Guid VehicleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public AvailabilityType Type { get; set; }
        public AvailabilityStatus Status { get; set; }
    }
}
