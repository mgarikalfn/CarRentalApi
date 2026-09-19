using Application.Dto.Booking;
using Application.Features.Bookings.Command;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalApi.Controllers;

/// <summary>Booking lifecycle management — create, approve, reject, cancel.</summary>
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
        _mediator    = mediator;
    }

    /// <summary>
    /// Create a new booking request for a vehicle.
    /// The authenticated user is automatically set as the renter.
    /// </summary>
    /// <response code="200">Booking created successfully.</response>
    /// <response code="400">Validation failed (dates invalid, vehicle unavailable, etc.).</response>
    /// <response code="401">Not authenticated.</response>
    [HttpPost]
    [ProducesResponseType(typeof(BookingDto), 200)]
    [ProducesResponseType(typeof(string), 400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<BookingDto>> CreateBooking(
        [FromBody] CreateBookingRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Unauthorized();

        // RenterId is set from JWT — client cannot supply or override it.
        var command = new CreateBookingCommand
        {
            VehicleId       = request.VehicleId,
            RenterId        = user.Id,
            StartDate       = request.StartDate,
            EndDate         = request.EndDate,
            PickUpLocation  = request.PickUpLocation,
            DropOffLocation = request.DropOffLocation
        };

        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// Cancel a pending or approved booking. Only the renter of the booking may cancel.
    /// </summary>
    /// <param name="id">Booking ID.</param>
    /// <param name="reason">Optional cancellation reason.</param>
    /// <response code="204">Cancelled successfully.</response>
    /// <response code="400">Cannot cancel in the current status, or not the renter.</response>
    /// <response code="401">Not authenticated.</response>
    [HttpPost("{id}/cancel")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(string), 400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> CancelBooking(Guid id, [FromBody] string? reason)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Unauthorized();

        var command = new CancelBookingCommand
        {
            BookingId = id,
            UserId    = user.Id,
            Reason    = reason ?? string.Empty
        };

        var result = await _mediator.Send(command);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    /// <summary>
    /// Approve a pending booking (host/admin action).
    /// </summary>
    /// <param name="id">Booking ID.</param>
    /// <response code="204">Approved successfully.</response>
    /// <response code="400">Booking is not in Pending state.</response>
    [HttpPost("{id}/approve")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<IActionResult> ApproveBooking(Guid id)
    {
        var command = new AcceptBookingCommand { BookingId = id };
        var result  = await _mediator.Send(command);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    /// <summary>
    /// Reject a pending booking (host/admin action).
    /// </summary>
    /// <param name="id">Booking ID.</param>
    /// <param name="reason">Rejection reason.</param>
    /// <response code="204">Rejected successfully.</response>
    /// <response code="400">Booking is not in Pending state.</response>
    [HttpPost("{id}/reject")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<IActionResult> RejectBooking(Guid id, [FromBody] string? reason)
    {
        var command = new RejectBookingCommand
        {
            BookingId = id,
            Reason    = reason ?? string.Empty
        };

        var result = await _mediator.Send(command);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}
