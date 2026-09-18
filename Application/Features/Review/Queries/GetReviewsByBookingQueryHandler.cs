using Application.Abstractions;
using Application.Common;
using Application.Dto.Review;
using AutoMapper;
using MediatR;

namespace Application.Features.Reviews.Queries;

public class GetReviewsByBookingQueryHandler
    : IRequestHandler<GetReviewsByBookingQuery, Result<List<ReviewDto>>>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IMapper _mapper;

    public GetReviewsByBookingQueryHandler(IReviewRepository reviewRepository, IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<ReviewDto>>> Handle(
        GetReviewsByBookingQuery request,
        CancellationToken cancellationToken)
    {
        var reviews = await _reviewRepository.GetByBookingIdAsync(request.BookingId, cancellationToken);
        var dtos = _mapper.Map<List<ReviewDto>>(reviews);
        return Result<List<ReviewDto>>.Success(dtos);
    }
}
