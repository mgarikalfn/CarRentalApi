using CarRentalApi.Application.Availability.Query;
using CarRentalApi.Data;
using CarRentalApi.Dto.Availablity;
using CarRentalApi.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediatR;
using CarRentalApi.Application.Availability.Command;
using Microsoft.AspNetCore.Identity;

namespace CarRentalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvailabilitesController : ControllerBase
    {
        private readonly RentalDbContext _context;
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;

        public AvailabilitesController(RentalDbContext context, IMediator mediator, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _mediator = mediator;
            _userManager = userManager;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Availability>>> GetAvailabilities(int id, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
           var command = new GetAvailabilityById
           {
               VehicleId = id,
               StartDate = startDate,
               EndDate = endDate
           };

            var result = await _mediator.Send(command);

            return result;
           
        }
    

    [HttpPost("{vehicleId}")]
        public async Task<ActionResult<Availability>> CreateAvailability([FromRoute] int vehicleId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var user = await _userManager.GetUserAsync(User);
            if(user == null) { return Unauthorized();
            }
            var command = new CreateAvailabilityCommand
            {
                OwnerId = user.Id,
                VehicleId = vehicleId,
                StartDate = startDate,
                EndDate = endDate
            };

            var result = await _mediator.Send(command);

           
            return Ok(result);
               
        }

        [HttpDelete("{vehicleId}")]
        public async Task<IActionResult> UnblockAvailability([FromRoute] int vehicleId, [FromQuery] int id)
        {
            var command = new DeleteAvailabilityCommand
            {
                VehicleId = vehicleId,
                Id = id
            };
            var result = await _mediator.Send(command);

            return result switch
            {
                1 => Ok(result),
                2 => BadRequest("availability status is already booked"),
                -1 => BadRequest("vehicle not found"),
                -2 => BadRequest("availability not found")
            };
        }
    }
}
