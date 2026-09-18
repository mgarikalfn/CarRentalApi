using Application.Common;
using Application.Dto.Booking;
using Domain.Enums;
using MediatR;

namespace Application.Features.Bookings.Command
{
    public class CreateBookingCommand : IRequest<Result<BookingDto>>
    {
        public Guid VehicleId { get; set; }
        public Guid RenterId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PickUpLocation { get; set; } = string.Empty;
        public string DropOffLocation { get; set; } = string.Empty;
    }
}
