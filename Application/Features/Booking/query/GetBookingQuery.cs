using Application.Dto.Booking;
using Domain.Enums;
using FluentResults;
using MediatR;

namespace Application.Features.Booking.Query
{
    public class GetBookingQuery : IRequest<Result<List<BookingDto>>>
    {
        public string UserId { get; set; }
        public int BookingId { get; set; }
        public string VehicleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Decimal TotalPrice { get; set; }
        public BookingStatus Status { get; set; }
    }
}
