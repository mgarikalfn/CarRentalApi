using CarRentalApi.Dto.Booking;
using CarRentalApi.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace CarRentalApi.Application.Booking.command
{
    public class CreateBookingCommand:IRequest<ActionResult<BookingDto>>
    {
              public int  VehicleId { get; set; }
              public string RenterId { get; set; }
              public DateTime  StartDate { get; set; }
              public DateTime   EndDate { get; set; }
              public Decimal TotalPrice { get; set; }
              public BookingStatus  Status { get; set; }
                
    }
}
