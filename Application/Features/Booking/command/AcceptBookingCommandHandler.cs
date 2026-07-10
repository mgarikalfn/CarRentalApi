using Application.Dto.Booking;
using Domain.Abstraction;
using Domain.Entities;
using Domain.Enums;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Booking.Command
{
    public class AcceptBookingCommandHandler : IRequestHandler<AcceptBookingCommand, Result<BookingDto>>
    {
        private readonly IBookingRepository _bookingRepository;

        public AcceptBookingCommandHandler(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<Result<BookingDto>> Handle(AcceptBookingCommand request, CancellationToken cancellationToken)
        {
            var booking = await _bookingRepository.GetByIdWithVehicleAsync(request.BookingId);

            if (booking == null)
            {
                return Result.Fail("Booking not found.");
            }

            if (booking.Vehicle.OwnerId != request.OwnerId)
            {
                return Result.Fail("Unauthorized to accept this booking.");
            }

            if (booking.Status != BookingStatus.Pending)
            {
                return Result.Fail("Only pending bookings can be accepted.");
            }

            booking.Status = BookingStatus.Confirmed;
            booking.UpdatedAt = DateTime.UtcNow;

            await _bookingRepository.UpdateAsync(booking);

            return Result.Ok(new BookingDto 
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
