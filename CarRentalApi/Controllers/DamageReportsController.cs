using Application.Dto.DamageReport;
using Application.Features.DamageReports.Commands;
using Application.Features.DamageReports.Queries;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalApi.Controllers;

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

    [HttpPost]
    public async Task<ActionResult<DamageReportDto>> CreateDamageReport(
        [FromBody] CreateDamageReportRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Unauthorized();

        // ReportedByUserId set from JWT -- never from client input
        var command = new CreateDamageReportCommand
        {
            BookingId          = request.BookingId,
            ReportedByUserId   = user.Id,
            Description        = request.Description,
            ImageUrls          = request.ImageUrls
        };

        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("{id}/start-review")]
    public async Task<ActionResult<DamageReportDto>> StartReview(Guid id)
    {
        var result = await _mediator.Send(new StartReviewCommand { DamageReportId = id });
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("{id}/resolve")]
    public async Task<ActionResult<DamageReportDto>> Resolve(Guid id, [FromQuery] bool atFault)
    {
        var result = await _mediator.Send(new ResolveCommand { DamageReportId = id, AtFault = atFault });
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("{id}/dismiss")]
    public async Task<ActionResult<DamageReportDto>> Dismiss(Guid id)
    {
        var result = await _mediator.Send(new DismissCommand { DamageReportId = id });
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DamageReportDto>> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetDamageReportByIdQuery { DamageReportId = id });
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpGet("booking/{bookingId}")]
    public async Task<ActionResult<List<DamageReportDto>>> GetByBooking(Guid bookingId)
    {
        var result = await _mediator.Send(new GetDamageReportsByBookingQuery { BookingId = bookingId });
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
