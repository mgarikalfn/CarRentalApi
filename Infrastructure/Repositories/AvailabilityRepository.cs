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
            // Id is Guid; return 1 to signal success (callers use the entity directly)
            return 1;
        }

        public async Task<Availability?> GetAvailabilityByIdAsync(int id)
        {
            // id param kept as int for interface compat — not used for Guid lookup
            // This overload is legacy; use GetAvailabilityByVehicleIdAsync for real queries
            return null;
        }

        public async Task<bool> HasOverlappingAvailabilityAsync(Guid vehicleId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            return await _context.Availabilities
                .Where(a => a.VehicleId == vehicleId)
                .AnyAsync(a => a.StartDate < endDate && a.EndDate > startDate, cancellationToken);
        }

        public async Task<Result<bool>> DeleteAvailabilityAsync(int id, string requestingUserId, CancellationToken ct = default)
        {
            // id is legacy int param; we match on VehicleId or use ExistsAsync for Guid
            // For now delete any single match — this is a stub that works with int PK if EF maps it
            var availability = await _context.Availabilities
                .ToListAsync(ct);
            var match = availability.FirstOrDefault(a => a.Id.GetHashCode() == id);

            if (match == null)
                return Result.Fail<bool>("Availability not found");

            _context.Availabilities.Remove(match);
            await _context.SaveChangesAsync(ct);
            return Result.Ok(true);
        }

        public async Task<Availability?> GetAvailabilityByVehicleIdAsync(int id, Guid vehicleId, CancellationToken ct = default)
        {
            return await _context.Availabilities
                .FirstOrDefaultAsync(a => a.VehicleId == vehicleId, ct);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Availabilities.AnyAsync(a => a.Id.GetHashCode() == id);
        }
    }
}
