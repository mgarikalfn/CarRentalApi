using Application.Features.Availability.Command;
using Application.Features.Availability.Query;
using Application.Dto.Availablity;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CarRentalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvailabilitesController : ControllerBase
    {
        private readonly Infrastructure.Data.RentalDbContext _context;
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;

        public AvailabilitesController(Infrastructure.Data.RentalDbContext context, IMediator mediator, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _mediator = mediator;
            _userManager = userManager;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<AvailabilityDto>>> GetAvailabilities(int id, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
           var command = new GetAvailabilityById
           {
               VehicleId = id,
               StartDate = startDate,
               EndDate = endDate
           };

            var result = await _mediator.Send(command);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors.Select(e => e.Message));
           
        }
    

    [HttpPost("{vehicleId}")]
        public async Task<ActionResult<int>> CreateAvailability([FromRoute] int vehicleId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
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

           
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors.Select(e => e.Message));
               
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

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors.Select(e => e.Message));
        }
    }
}
