using Domain.Abstraction;
using Domain.Entities;
using FluentResults;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class AvailabilityRepository : IAvailabilityRepository
    {
        private readonly RentalDbContext _context;

        public AvailabilityRepository(RentalDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateAvailabilityRepository(Availability availability)
        {
            _context.Availabilities.Add(availability);
            await _context.SaveChangesAsync();
            return availability.Id;
        }

        public async Task<Availability?> GetAvailabilityByIdAsync(int id)
        {
            return await _context.Availabilities
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<bool> HasOverlappingAvailabilityAsync(int vehicleId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            return await _context.Availabilities
                .Where(a => a.VehicleId == vehicleId)
                .AnyAsync(a => a.StartDate < endDate && a.EndDate > startDate, cancellationToken);
        }

        public async Task<Result<bool>> DeleteAvailabilityAsync(int id, string requestingUserId, CancellationToken ct = default)
        {
            var availability = await _context.Availabilities
                .FirstOrDefaultAsync(a => a.Id == id, ct);

            if (availability == null)
            {
                return Result.Fail<bool>("Availability not found");
            }

            _context.Availabilities.Remove(availability);
            await _context.SaveChangesAsync(ct);

            return Result.Ok(true);
        }

        public async Task<Availability?> GetAvailabilityByVehicleIdAsync(int vehicleId, int id, CancellationToken ct = default)
        {
            return await _context.Availabilities
                .FirstOrDefaultAsync(a => a.Id == id && a.VehicleId == vehicleId, ct);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Availabilities.AnyAsync(a => a.Id == id);
        }
    }
}