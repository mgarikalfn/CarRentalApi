using Application.Abstractions;
using Application.Common;
using Application.Dto.Review;
using AutoMapper;
using Domain.Common;
using MediatR;

namespace Application.Features.Reviews.Commands;

public class HideReviewCommandHandler
    : IRequestHandler<HideReviewCommand, Result<ReviewDto>>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IMapper _mapper;

    public HideReviewCommandHandler(IReviewRepository reviewRepository, IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _mapper = mapper;
    }

    public async Task<Result<ReviewDto>> Handle(
        HideReviewCommand request,
        CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetByIdAsync(request.ReviewId, cancellationToken);
        if (review is null)
            return Result<ReviewDto>.Failure("Review not found.");

        try
        {
            review.Hide();
        }
        catch (DomainException ex)
        {
            return Result<ReviewDto>.Failure(ex.Message);
        }

        await _reviewRepository.UpdateAsync(review, cancellationToken);
        return Result<ReviewDto>.Success(_mapper.Map<ReviewDto>(review));
    }
}
