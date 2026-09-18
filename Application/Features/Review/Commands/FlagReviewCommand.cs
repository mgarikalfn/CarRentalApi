using Application.Common;
using Application.Dto.Review;
using MediatR;

namespace Application.Features.Reviews.Commands;

public class FlagReviewCommand : IRequest<Result<ReviewDto>>
{
    public Guid ReviewId { get; set; }
}
