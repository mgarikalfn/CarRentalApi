using Application.Features.Availabilities.Command;
using Application.Features.Availabilities.Query;
using Application.Dto.Availablity;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CarRentalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvailabilitesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;

        public AvailabilitesController(IMediator mediator, UserManager<ApplicationUser> userManager)
        {
            _mediator = mediator;
            _userManager = userManager;
        }

        [HttpGet("{vehicleId}")]
        public async Task<ActionResult<IEnumerable<AvailabilityDto>>> GetAvailabilities(
            Guid vehicleId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var command = new GetAvailabilityById
            {
                VehicleId = vehicleId,
                StartDate = startDate,
                EndDate = endDate
            };
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
        }

        [HttpPost("{vehicleId}")]
        [Authorize]
        public async Task<ActionResult<int>> CreateAvailability(
            [FromRoute] Guid vehicleId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] Domain.Enums.AvailabilityType type = Domain.Enums.AvailabilityType.Booking)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var command = new CreateAvailabilityCommand
            {
                OwnerId = user.Id,
                VehicleId = vehicleId,
                StartDate = startDate,
                EndDate = endDate,
                Type = type
            };
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpDelete("{vehicleId}")]
        [Authorize]
        public async Task<IActionResult> DeleteAvailability(
            [FromRoute] Guid vehicleId,
            [FromQuery] int id)
        {
            var command = new DeleteAvailabilityCommand
            {
                VehicleId = vehicleId,
                Id = id
            };
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }
    }
}
