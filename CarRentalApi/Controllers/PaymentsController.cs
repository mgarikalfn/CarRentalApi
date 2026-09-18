using Application.Dto.Payment;
using Application.Features.Payments.Commands;
using Application.Features.Payments.Queries;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly UserManager<ApplicationUser> _userManager;

    public PaymentsController(IMediator mediator, UserManager<ApplicationUser> userManager)
    {
        _mediator = mediator;
        _userManager = userManager;
    }

    /// <summary>
    /// Initiate a payment for an approved booking.
    /// The authenticated user is automatically set as the payer.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<PaymentDto>> CreatePayment([FromBody] CreatePaymentCommand command)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Unauthorized();

        command.PayerId = user.Id;
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// Mark a payment as succeeded (admin/webhook use).
    /// </summary>
    [HttpPost("{id}/succeed")]
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
    [HttpPost("{id}/fail")]
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
    [HttpPost("{id}/refund")]
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
    [HttpGet("{id}")]
    public async Task<ActionResult<PaymentDto>> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetPaymentByIdQuery { PaymentId = id });
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    /// <summary>
    /// Get all payments associated with a booking.
    /// </summary>
    [HttpGet("booking/{bookingId}")]
    public async Task<ActionResult<List<PaymentDto>>> GetByBooking(Guid bookingId)
    {
        var result = await _mediator.Send(new GetPaymentsByBookingQuery { BookingId = bookingId });
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
