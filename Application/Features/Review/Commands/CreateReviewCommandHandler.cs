using Application.Abstractions;
using Application.Common;
using Application.Dto.Review;
using AutoMapper;
using Domain.Abstraction;
using Domain.Common;
using Domain.Enums;
using MediatR;
using DomainReview = Domain.Entities.Review;

namespace Application.Features.Reviews.Commands;

public class CreateReviewCommandHandler
    : IRequestHandler<CreateReviewCommand, Result<ReviewDto>>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMapper _mapper;

    public CreateReviewCommandHandler(
        IReviewRepository reviewRepository,
        IBookingRepository bookingRepository,
        IVehicleRepository vehicleRepository,
        IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _bookingRepository = bookingRepository;
        _vehicleRepository = vehicleRepository;
        _mapper = mapper;
    }

    public async Task<Result<ReviewDto>> Handle(
        CreateReviewCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Load booking — must exist
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId);
        if (booking is null)
            return Result<ReviewDto>.Failure("Booking not found.");

        // 2. Booking must be in Completed status (cross-aggregate check lives here, not in Review)
        if (booking.Status != BookingStatus.Completed)
            return Result<ReviewDto>.Failure("A review can only be submitted for a completed booking.");

        // 3. Load vehicle to determine host identity at query time (Option A per design §12)
        var vehicle = await _vehicleRepository.GetVehicleByIdAsync(booking.VehicleId);
        if (vehicle is null)
            return Result<ReviewDto>.Failure("Vehicle not found.");

        var renterId = booking.RenterId;
        var hostId = vehicle.OwnerId;

        // 4. Derive revieweeId — only a party to the booking may review
        //    Renter reviews host; host reviews renter; no other pairing is valid.
        //    RevieweeId is server-derived: the client does NOT supply it.
        Guid revieweeId;
        if (request.ReviewerId == renterId)
        {
            revieweeId = hostId;
        }
        else if (request.ReviewerId == hostId)
        {
            revieweeId = renterId;
        }
        else
        {
            return Result<ReviewDto>.Failure("Only a party to the booking can submit a review.");
        }

        // 5. Create domain aggregate (validates rating range, comment length, non-empty GUIDs)
        DomainReview review;
        try
        {
            review = DomainReview.Create(
                booking.Id,
                booking.VehicleId,
                request.ReviewerId,
                revieweeId,          // server-derived, not from client input
                request.Rating,
                request.Comment);
        }
        catch (DomainException ex)
        {
            return Result<ReviewDto>.Failure(ex.Message);
        }

        // 6. Persist — DuplicateReviewException is translated from PostgresException in repo
        try
        {
            await _reviewRepository.AddAsync(review, cancellationToken);
        }
        catch (DuplicateReviewException ex)
        {
            return Result<ReviewDto>.Failure(ex.Message);
        }

        return Result<ReviewDto>.Success(_mapper.Map<ReviewDto>(review));
    }
}
