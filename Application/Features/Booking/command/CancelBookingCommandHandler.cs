using Application.Dto.Booking;
using Domain.Abstraction;
using Domain.Entities;
using Domain.Enums;
using FluentResults;
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
            var booking = await _bookingRepository.GetByIdWithVehicleAsync(request.BookingId);

            if (booking == null)
            {
                return Result.Fail("Booking not found.");
            }

            if (booking.RenterId != request.UserId && booking.Vehicle.OwnerId != request.UserId)
            {
                return Result.Fail("Unauthorized to cancel this booking.");
            }

            if (booking.Status != BookingStatus.Pending && booking.Status != BookingStatus.Confirmed)
            {
                return Result.Fail("Booking cannot be cancelled in its current status.");
            }

            booking.Status = BookingStatus.Cancelled;
            booking.UpdatedAt = DateTime.UtcNow;

            await _bookingRepository.UpdateAsync(booking);

            return Result.Ok(new BookingDto { Id = booking.Id, VehicleId = booking.VehicleId, StartDate = booking.StartDate, EndDate = booking.EndDate, Status = booking.Status });
        }
    }
}
