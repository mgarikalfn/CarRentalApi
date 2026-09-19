using Application.Dto.Payment;
using Application.Features.Payments.Commands;
using Application.Features.Payments.Queries;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalApi.Controllers;

/// <summary>Payment lifecycle — initiate, succeed, fail, refund.</summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly UserManager<ApplicationUser> _userManager;

    public PaymentsController(IMediator mediator, UserManager<ApplicationUser> userManager)
    {
        _mediator    = mediator;
        _userManager = userManager;
    }

    /// <summary>
    /// Initiate a payment for an approved booking.
    /// The authenticated user is automatically set as the payer.
    /// Amount must exactly match the booking's total price.
    /// </summary>
    /// <response code="200">Payment initiated successfully (status: Pending).</response>
    /// <response code="400">Amount mismatch, booking not approved, or duplicate active payment.</response>
    /// <response code="401">Not authenticated.</response>
    [HttpPost]
    [ProducesResponseType(typeof(PaymentDto), 200)]
    [ProducesResponseType(typeof(string), 400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<PaymentDto>> CreatePayment(
        [FromBody] CreatePaymentRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Unauthorized();

        // PayerId is set from JWT — client cannot supply or override it.
        var command = new CreatePaymentCommand
        {
            BookingId = request.BookingId,
            PayerId   = user.Id,
            Amount    = request.Amount,
            Method    = request.Method
        };

        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// Mark a payment as succeeded (admin/webhook use).
    /// Provide the gateway transaction reference in the request body.
    /// </summary>
    /// <param name="id">Payment ID.</param>
    /// <param name="command">Request body containing the transaction reference.</param>
    /// <response code="200">Payment marked as Succeeded.</response>
    /// <response code="400">Payment is not in Pending state.</response>
    [HttpPost("{id}/succeed")]
    [ProducesResponseType(typeof(PaymentDto), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<ActionResult<PaymentDto>> MarkSucceeded(
        Guid id,
        [FromBody] MarkPaymentSucceededCommand command)
    {
        command.PaymentId = id;
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// Mark a payment as failed (admin/webhook use).
    /// </summary>
    /// <param name="id">Payment ID.</param>
    /// <param name="command">Request body containing the failure reason.</param>
    /// <response code="200">Payment marked as Failed.</response>
    /// <response code="400">Payment is not in Pending state.</response>
    [HttpPost("{id}/fail")]
    [ProducesResponseType(typeof(PaymentDto), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<ActionResult<PaymentDto>> MarkFailed(
        Guid id,
        [FromBody] MarkPaymentFailedCommand command)
    {
        command.PaymentId = id;
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// Refund a succeeded payment.
    /// </summary>
    /// <param name="id">Payment ID.</param>
    /// <param name="command">Request body containing the refund reason.</param>
    /// <response code="200">Payment refunded successfully.</response>
    /// <response code="400">Payment is not in Succeeded state.</response>
    [HttpPost("{id}/refund")]
    [ProducesResponseType(typeof(PaymentDto), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<ActionResult<PaymentDto>> Refund(
        Guid id,
        [FromBody] RefundPaymentCommand command)
    {
        command.PaymentId = id;
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// Get a payment by its ID.
    /// </summary>
    /// <param name="id">Payment ID.</param>
    /// <response code="200">Payment found.</response>
    /// <response code="404">Payment not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PaymentDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<PaymentDto>> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetPaymentByIdQuery { PaymentId = id });
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    /// <summary>
    /// Get all payments associated with a booking.
    /// </summary>
    /// <param name="bookingId">Booking ID.</param>
    /// <response code="200">List of payments (may be empty).</response>
    [HttpGet("booking/{bookingId}")]
    [ProducesResponseType(typeof(List<PaymentDto>), 200)]
    public async Task<ActionResult<List<PaymentDto>>> GetByBooking(Guid bookingId)
    {
        var result = await _mediator.Send(new GetPaymentsByBookingQuery { BookingId = bookingId });
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
