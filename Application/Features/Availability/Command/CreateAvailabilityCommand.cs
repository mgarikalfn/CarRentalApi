using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;
using FluentResults;
using MediatR;

namespace Application.Features.Availability.Command
{
    public class CreateAvailabilityCommand:IRequest<Result<int>>
    {
        public string OwnerId { get; set; }
        public int VehicleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public AvailabilityStatus Status { get; set; } // Available, Blocked, Booked
    }
}
