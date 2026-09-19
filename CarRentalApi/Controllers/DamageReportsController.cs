using Application.Dto.DamageReport;
using Application.Features.DamageReports.Commands;
using Application.Features.DamageReports.Queries;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalApi.Controllers;

/// <summary>Damage report lifecycle — file, review, resolve, dismiss.</summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DamageReportsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly UserManager<ApplicationUser> _userManager;

    public DamageReportsController(IMediator mediator, UserManager<ApplicationUser> userManager)
    {
        _mediator    = mediator;
        _userManager = userManager;
    }

    /// <summary>
    /// File a damage report for an active or completed booking.
    /// The authenticated user is automatically set as the reporter.
    /// Both the renter and the host of the booking may file a report.
    /// Up to 10 image URLs may be attached.
    /// </summary>
    /// <response code="200">Damage report created (status: Open).</response>
    /// <response code="400">Booking not found, reporter not a party, or validation error.</response>
    /// <response code="401">Not authenticated.</response>
    [HttpPost]
    [ProducesResponseType(typeof(DamageReportDto), 200)]
    [ProducesResponseType(typeof(string), 400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<DamageReportDto>> CreateDamageReport(
        [FromBody] CreateDamageReportRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Unauthorized();

        // ReportedByUserId set from JWT — client cannot supply or override it.
        var command = new CreateDamageReportCommand
        {
            BookingId        = request.BookingId,
            ReportedByUserId = user.Id,
            Description      = request.Description,
            ImageUrls        = request.ImageUrls
        };

        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// Move an Open damage report to UnderReview (admin action).
    /// </summary>
    /// <param name="id">Damage report ID.</param>
    /// <response code="200">Report is now UnderReview.</response>
    /// <response code="400">Report is not in Open state.</response>
    [HttpPost("{id}/start-review")]
    [ProducesResponseType(typeof(DamageReportDto), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<ActionResult<DamageReportDto>> StartReview(Guid id)
    {
        var result = await _mediator.Send(new StartReviewCommand { DamageReportId = id });
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// Resolve a report under review (admin action).
    /// Pass atFault=true for ResolvedAtFault, atFault=false for ResolvedNotAtFault.
    /// Raises DamageReportResolvedEvent for future TrustProfile recalculation.
    /// </summary>
    /// <param name="id">Damage report ID.</param>
    /// <param name="atFault">Whether the reporter is determined to be at fault.</param>
    /// <response code="200">Report resolved.</response>
    /// <response code="400">Report is not in UnderReview state.</response>
    [HttpPost("{id}/resolve")]
    [ProducesResponseType(typeof(DamageReportDto), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<ActionResult<DamageReportDto>> Resolve(Guid id, [FromQuery] bool atFault)
    {
        var result = await _mediator.Send(new ResolveCommand { DamageReportId = id, AtFault = atFault });
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// Dismiss a report under review without a fault determination (admin action).
    /// Does not raise DamageReportResolvedEvent.
    /// </summary>
    /// <param name="id">Damage report ID.</param>
    /// <response code="200">Report dismissed.</response>
    /// <response code="400">Report is not in UnderReview state.</response>
    [HttpPost("{id}/dismiss")]
    [ProducesResponseType(typeof(DamageReportDto), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<ActionResult<DamageReportDto>> Dismiss(Guid id)
    {
        var result = await _mediator.Send(new DismissCommand { DamageReportId = id });
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// Get a damage report by its ID.
    /// </summary>
    /// <param name="id">Damage report ID.</param>
    /// <response code="200">Report found.</response>
    /// <response code="404">Report not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(DamageReportDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<DamageReportDto>> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetDamageReportByIdQuery { DamageReportId = id });
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    /// <summary>
    /// Get all damage reports for a booking.
    /// Multiple open reports per booking are permitted (e.g. renter and host each report different damage).
    /// </summary>
    /// <param name="bookingId">Booking ID.</param>
    /// <response code="200">List of damage reports.</response>
    /// <response code="400">No reports found for this booking.</response>
    [HttpGet("booking/{bookingId}")]
    [ProducesResponseType(typeof(List<DamageReportDto>), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<ActionResult<List<DamageReportDto>>> GetByBooking(Guid bookingId)
    {
        var result = await _mediator.Send(new GetDamageReportsByBookingQuery { BookingId = bookingId });
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
