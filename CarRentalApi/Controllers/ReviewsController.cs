using Application.Dto.Review;
using Application.Features.Reviews.Commands;
using Application.Features.Reviews.Queries;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ReviewsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly UserManager<ApplicationUser> _userManager;

    public ReviewsController(IMediator mediator, UserManager<ApplicationUser> userManager)
    {
        _mediator = mediator;
        _userManager = userManager;
    }

    /// <summary>
    /// Submit a review for a completed booking.
    /// The authenticated user is automatically set as the reviewer.
    /// The reviewee is derived server-side from booking participants.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ReviewDto>> CreateReview(
        [FromBody] CreateReviewRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Unauthorized();

        // ReviewerId comes from JWT identity — not from the request body.
        // RevieweeId is derived inside the handler from booking participants.
        var command = new CreateReviewCommand
        {
            BookingId  = request.BookingId,
            ReviewerId = user.Id,          // ← server-set, not client input
            Rating     = request.Rating,
            Comment    = request.Comment
        };

        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// Flag a review for moderator attention.
    /// Any authenticated user may call this. Does NOT hide the review.
    /// </summary>
    [HttpPost("{id}/flag")]
    public async Task<ActionResult<ReviewDto>> FlagReview(Guid id)
    {
        var result = await _mediator.Send(new FlagReviewCommand { ReviewId = id });
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// Hide a review from public view (admin only).
    /// </summary>
    [HttpPost("{id}/hide")]
    public async Task<ActionResult<ReviewDto>> HideReview(Guid id)
    {
        var result = await _mediator.Send(new HideReviewCommand { ReviewId = id });
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// Restore a hidden or flagged review to published/clean state (admin only).
    /// </summary>
    [HttpPost("{id}/restore")]
    public async Task<ActionResult<ReviewDto>> RestoreReview(Guid id)
    {
        var result = await _mediator.Send(new RestoreReviewCommand { ReviewId = id });
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// Get a review by its ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ReviewDto>> GetReviewById(Guid id)
    {
        var result = await _mediator.Send(new GetReviewByIdQuery { ReviewId = id });
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    /// <summary>
    /// Get all reviews for a specific booking (up to two — one per direction).
    /// </summary>
    [HttpGet("booking/{bookingId}")]
    public async Task<ActionResult<List<ReviewDto>>> GetReviewsByBooking(Guid bookingId)
    {
        var result = await _mediator.Send(new GetReviewsByBookingQuery { BookingId = bookingId });
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// Get all reviews received by a specific user (reviews where they are the reviewee).
    /// </summary>
    [HttpGet("reviewee/{revieweeId}")]
    public async Task<ActionResult<List<ReviewDto>>> GetReviewsByReviewee(Guid revieweeId)
    {
        var result = await _mediator.Send(new GetReviewsByRevieweeQuery { RevieweeId = revieweeId });
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
