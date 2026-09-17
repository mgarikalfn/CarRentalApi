using Domain.Abstraction;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly RentalDbContext _context;

        public BookingRepository(RentalDbContext context)
        {
            _context = context;
        }

        public async Task<Booking?> GetByIdAsync(Guid id)
        {
            return await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Booking>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Bookings
                .Where(b => b.RenterId == userId)
                .OrderByDescending(b => b.StartDate)
                .ToListAsync();
        }

        public async Task<bool> IsVehicleBookedAsync(Guid vehicleId, DateTime startDate, DateTime endDate)
        {
            return await _context.Bookings
                .AnyAsync(b => b.VehicleId == vehicleId &&
                               b.Status != BookingStatus.Cancelled &&
                               b.Status != BookingStatus.Rejected &&
                               ((startDate >= b.StartDate && startDate <= b.EndDate) ||
                                (endDate >= b.StartDate && endDate <= b.EndDate) ||
                                (startDate <= b.StartDate && endDate >= b.EndDate)));
        }

        public async Task AddAsync(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
        }
    }
}
