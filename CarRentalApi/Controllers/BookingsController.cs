using Application.Common;
using Application.Dto.Booking;
using Application.Features.Booking.Command;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace CarRentalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMediator _mediator;

        public BookingsController(UserManager<ApplicationUser> userManager, IMediator mediator)
        {
            _userManager = userManager;
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<Result<BookingDto>>> CreateBooking([FromBody] CreateBookingCommand command)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();
            command.RenterId = user.Id;
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelBooking(Guid id, [FromBody] string? reason)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();
            var command = new CancelBookingCommand { BookingId = id, UserId = user.Id, Reason = reason ?? string.Empty };
            var result = await _mediator.Send(command);
            return result.IsSuccess ? NoContent() : BadRequest(result.Error);
        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveBooking(Guid id)
        {
            var command = new AcceptBookingCommand { BookingId = id };
            var result = await _mediator.Send(command);
            return result.IsSuccess ? NoContent() : BadRequest(result.Error);
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectBooking(Guid id, [FromBody] string? reason)
        {
            var command = new RejectBookingCommand { BookingId = id, Reason = reason ?? string.Empty };
            var result = await _mediator.Send(command);
            return result.IsSuccess ? NoContent() : BadRequest(result.Error);
        }
    }
}
