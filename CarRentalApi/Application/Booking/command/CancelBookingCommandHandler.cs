using CarRentalApi.Data;
using CarRentalApi.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRentalApi.Application.Booking.command
{
    public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand,ActionResult>
    {
        private readonly RentalDbContext _context;
        public CancelBookingCommandHandler(RentalDbContext context)
        {
            _context = context;
        }
        public async  Task<ActionResult> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
        {
            
            var booking = await _context.Bookings
                .Include(b => b.Vehicle)
                .FirstOrDefaultAsync(b => b.Id == request.id &&
                    (b.RenterId == request.UserId || b.Vehicle.OwnerId == request.UserId));

            if (booking == null)
            {
                return new NotFoundResult();
            }

            if (booking.Status != BookingStatus.Pending && booking.Status != BookingStatus.Confirmed)
            {
                return new BadRequestObjectResult("Booking can't be cancelled at this stage");
            }

            booking.Status = BookingStatus.Cancelled;
            booking.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return new NoContentResult();
        }
    }
}
