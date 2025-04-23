using CarRentalApi.Data;
using CarRentalApi.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRentalApi.Application.Booking.command
{
    public class RejectBookingCommandHandler:IRequestHandler<CancelBookingCommand, ActionResult>
    {
        private readonly RentalDbContext _context;
        public RejectBookingCommandHandler(RentalDbContext context)
        {
            _context = context;
        }
        public async Task<ActionResult> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
        {
            var booking = await _context.Bookings
                 .Include(b => b.Vehicle)
                 .FirstOrDefaultAsync(b => b.Id == request.id &&
                 b.Vehicle.OwnerId == request.UserId);

            if (booking == null)
            {
                return new NotFoundResult();
            }



            booking.Status = BookingStatus.Rejected;
            booking.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return new  NoContentResult();
        }
    }
    
}
