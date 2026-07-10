using Application.Dto.Booking;
using Domain.Enums;
using FluentResults;
using MediatR;

namespace Application.Features.Booking.Command
{
    public class CreateBookingCommand : IRequest<Result<BookingDto>>
    {
        public int VehicleId { get; set; }
        public string RenterId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
