using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalApi.Application.Availability.Query
{
    public class GetAvailabilityById:IRequest<ActionResult>
    {
        [Required]
        public int VehicleId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

    }
}
