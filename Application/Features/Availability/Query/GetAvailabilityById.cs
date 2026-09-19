using System.ComponentModel.DataAnnotations;
using Application.Common;
using Application.Dto.Availablity;
using MediatR;

namespace Application.Features.Availabilities.Query
{
    public class GetAvailabilityById : IRequest<Result<List<AvailabilityDto>>>
    {
        [Required]
        public Guid VehicleId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
