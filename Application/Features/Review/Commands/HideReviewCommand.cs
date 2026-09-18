using Application.Common;
using Application.Dto.Review;
using MediatR;

namespace Application.Features.Reviews.Commands;

public class HideReviewCommand : IRequest<Result<ReviewDto>>
{
    public Guid ReviewId { get; set; }
}
