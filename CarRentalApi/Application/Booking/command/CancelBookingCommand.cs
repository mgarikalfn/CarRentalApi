using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalApi.Application.Booking.command
{
    public class CancelBookingCommand:IRequest<ActionResult>
    {
        public int id { get; set; }
        public string UserId { get; set; }
    }
}
