using Application.Common;
using Application.Dto.Review;
using MediatR;

namespace Application.Features.Reviews.Commands;

public class CreateReviewCommand : IRequest<Result<ReviewDto>>
{
    /// <summary>The booking this review is for.</summary>
    public Guid BookingId { get; set; }

    /// <summary>
    /// Set by the controller from the authenticated user's JWT identity.
    /// The client does NOT supply this field.
    /// </summary>
    public Guid ReviewerId { get; set; }

    public int Rating { get; set; }
    public string? Comment { get; set; }
}
