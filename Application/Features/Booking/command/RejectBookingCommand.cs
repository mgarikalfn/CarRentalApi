using Application.Common;
using Application.Dto.Booking;
using MediatR;

namespace Application.Features.Booking.Command
{
    public class RejectBookingCommand : IRequest<Result<BookingDto>>
    {
        public Guid BookingId { get; set; }
        public Guid OwnerId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
