using Application.Common;
using Application.Dto.Booking;
using MediatR;

namespace Application.Features.Booking.Command
{
    public class CancelBookingCommand : IRequest<Result<BookingDto>>
    {
        public Guid BookingId { get; set; }
        public Guid UserId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
