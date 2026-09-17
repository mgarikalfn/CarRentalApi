using System.ComponentModel.DataAnnotations;
using Application.Dto.Availablity;
using FluentResults;
using MediatR;

namespace Application.Features.Availability.Query
{
    public class GetAvailabilityById : IRequest<Result<List<AvailabilityDto>>>
    {
        [Required]
        public Guid VehicleId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

    }
}
