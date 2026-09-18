using Application.Abstractions;
using Application.Common;
using Application.Dto.Review;
using AutoMapper;
using MediatR;

namespace Application.Features.Reviews.Queries;

public class GetReviewByIdQueryHandler
    : IRequestHandler<GetReviewByIdQuery, Result<ReviewDto>>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IMapper _mapper;

    public GetReviewByIdQueryHandler(IReviewRepository reviewRepository, IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _mapper = mapper;
    }

    public async Task<Result<ReviewDto>> Handle(
        GetReviewByIdQuery request,
        CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetByIdAsync(request.ReviewId, cancellationToken);
        if (review is null)
            return Result<ReviewDto>.Failure("Review not found.");

        return Result<ReviewDto>.Success(_mapper.Map<ReviewDto>(review));
    }
}
