using Application.Common;
using Application.Dto.Booking;
using Domain.Abstraction;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features.Booking.Command
{
    public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, Result<BookingDto>>
    {
        private readonly IBookingRepository _bookingRepository;

        public CancelBookingCommandHandler(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<Result<BookingDto>> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
        {
            var booking = await _bookingRepository.GetByIdAsync(request.BookingId);

            if (booking == null)
            {
                return Result<BookingDto>.Failure("Booking not found.");
            }

            if (booking.RenterId != request.UserId)
            {
                return Result<BookingDto>.Failure("Unauthorized to cancel this booking.");
            }

            if (booking.Status != BookingStatus.Pending && booking.Status != BookingStatus.Approved)
            {
                return Result<BookingDto>.Failure("Booking cannot be cancelled in its current status.");
            }

            booking.Cancel(request.Reason ?? "Cancelled by renter");

            await _bookingRepository.UpdateAsync(booking);

            return Result<BookingDto>.Success(new BookingDto 
            { 
                Id = booking.Id, 
                VehicleId = booking.VehicleId, 
                StartDate = booking.StartDate, 
                EndDate = booking.EndDate, 
                Status = booking.Status 
            });
        }
    }
}
