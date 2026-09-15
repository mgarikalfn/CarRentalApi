using Application.Common;
using Application.Dto.Booking;
using MediatR;

namespace Application.Features.Booking.Command
{
    public class AcceptBookingCommand : IRequest<Result<BookingDto>>
    {
        public Guid BookingId { get; set; }
        public Guid OwnerId { get; set; }
    }
}
