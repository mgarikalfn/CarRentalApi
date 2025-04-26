using System.Threading;
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
        public Task<bool> AvailabilityExistsAsync(string licensePlate)
        {
            throw new NotImplementedException();
        }

        public async Task<int> CreateAvailabilityRepository(Availability availability)
        {
            _context.Availabilities.Add(availability);
            await _context.SaveChangesAsync();
            return availability.Id;
        }

        public Task<Result<bool>> DeleteAvailabilityAsync(int id, string requestingUserId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Availability?> GetAvailabilityByIdAsync(int id)
        {
            throw new NotImplementedException();
        }


     

        public async Task<bool> HasOverlappingAvailabilityAsync(int vehicleId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
        {
            return await _context.Availabilities
                .Where(a => a.VehicleId == vehicleId)
                .AnyAsync(a => a.StartDate < endDate && a.EndDate > startDate, cancellationToken);
        }

        async Task<Result<bool>> IAvailabilityRepository.DeleteAvailabilityAsync(Availability availability)
        {
             _context.Availabilities.Remove(availability);
            await _context.SaveChangesAsync();
            return Result<bool>

        }

        Task<Result<bool>> IAvailabilityRepository.DeleteAvailabilityAsync(int id, string requestingUserId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        async Task<Availability> IAvailabilityRepository.GetAvailabilityByVehicleIdAsync(int vehicleId, int id, CancellationToken cancellationToken)
        {
            return await _context.Availabilities
                 .FirstOrDefaultAsync(a => a.Id == id && a.VehicleId == vehicleId, cancellationToken);
        }
    }
}
