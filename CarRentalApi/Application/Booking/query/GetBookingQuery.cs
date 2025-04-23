using CarRentalApi.Dto.Booking;
using CarRentalApi.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalApi.Application.Booking.query
{
    public class GetBookingQuery : IRequest<ActionResult<List<BookingDto>>>
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
