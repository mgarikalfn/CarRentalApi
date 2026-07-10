using Application.Dto.Booking;
using FluentResults;
using MediatR;

namespace Application.Features.Booking.Command
{
    public class AcceptBookingCommand : IRequest<Result<BookingDto>>
    {
        public int BookingId { get; set; }
        public string OwnerId { get; set; }
    }
}
