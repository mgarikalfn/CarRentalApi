using Application.Dto.Booking;
using FluentResults;
using MediatR;

namespace Application.Features.Booking.Command
{
    public class CancelBookingCommand : IRequest<Result<BookingDto>>
    {
        public int BookingId { get; set; }
        public string UserId { get; set; }
    }
}
