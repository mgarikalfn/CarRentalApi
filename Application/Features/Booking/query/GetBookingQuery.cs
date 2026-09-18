using Application.Common;
using Application.Dto.Booking;
using Domain.Enums;
using MediatR;

namespace Application.Features.Bookings.Query
{
    public class GetBookingQuery : IRequest<Result<List<BookingDto>>>
    {
        public Guid UserId { get; set; }
        public BookingStatus? StatusFilter { get; set; }
    }
}
