using Application.Dto.Booking;
using Domain.Abstraction;
using Domain.Entities;
using Domain.Enums;
using FluentResults;
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
            var booking = await _bookingRepository.GetByIdWithVehicleAsync(request.BookingId);

            if (booking == null)
            {
                return Result.Fail("Booking not found.");
            }

            if (booking.Vehicle.OwnerId != request.OwnerId)
            {
                return Result.Fail("Unauthorized to reject this booking.");
            }

            if (booking.Status != BookingStatus.Pending)
            {
                return Result.Fail("Only pending bookings can be rejected.");
            }

            booking.Status = BookingStatus.Rejected;
            booking.UpdatedAt = DateTime.UtcNow;

            await _bookingRepository.UpdateAsync(booking);

            return Result.Ok(new BookingDto { Id = booking.Id, VehicleId = booking.VehicleId, StartDate = booking.StartDate, EndDate = booking.EndDate, Status = booking.Status });
        }
    }
}
