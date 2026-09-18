using Application.Common;
using Application.Dto.Review;
using MediatR;

namespace Application.Features.Reviews.Queries;

public class GetReviewsByRevieweeQuery : IRequest<Result<List<ReviewDto>>>
{
    public Guid RevieweeId { get; set; }
}
