using Application.Common;
using Application.Dto.Review;
using MediatR;

namespace Application.Features.Reviews.Queries;

public class GetReviewByIdQuery : IRequest<Result<ReviewDto>>
{
    public Guid ReviewId { get; set; }
}
