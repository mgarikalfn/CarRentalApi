using Application.Common;
using Application.Dto.Review;
using MediatR;

namespace Application.Features.Reviews.Queries;

public class GetReviewsByBookingQuery : IRequest<Result<List<ReviewDto>>>
{
    public Guid BookingId { get; set; }
}
