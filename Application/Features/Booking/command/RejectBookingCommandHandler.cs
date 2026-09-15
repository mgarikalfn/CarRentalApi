using Application.Common;
using Application.Dto.Booking;
using Domain.Abstraction;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features.Booking.Command
{
    public class RejectBookingCommandHandler : IRequestHandler<RejectBookingCommand, Result<BookingDto>>
    {
        private readonly IBookingRepository _bookingRepository;

        public RejectBookingCommandHandler(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<Result<BookingDto>> Handle(RejectBookingCommand request, CancellationToken cancellationToken)
        {
            var booking = await _bookingRepository.GetByIdAsync(request.BookingId);

            if (booking == null)
            {
                return Result<BookingDto>.Failure("Booking not found.");
            }

            if (booking.Status != BookingStatus.Pending)
            {
                return Result<BookingDto>.Failure("Only pending bookings can be rejected.");
            }

            booking.Reject(request.Reason ?? "No reason provided");

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
