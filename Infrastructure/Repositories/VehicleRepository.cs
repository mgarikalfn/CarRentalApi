using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Abstraction;
using Domain.Entities;
using Domain.Common;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Domain.Services.Interfaces;

namespace Infrastructure.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly RentalDbContext _context;
        private readonly IFileStorageService _fileStorageService;

        public VehicleRepository(RentalDbContext context, IFileStorageService fileStorageService)
        {
            _context = context;
            _fileStorageService = fileStorageService;
        }

        public async Task<int> CreateVehicleAsync(Vehicle vehicle)
        {
            await _context.Vehicles.AddAsync(vehicle);
            await _context.SaveChangesAsync();
            // Return 1 to indicate success; callers use the vehicle.Id Guid directly if needed
            return 1;
        }

        public async Task<Vehicle?> GetVehicleByIdAsync(Guid id)
        {
            return await _context.Vehicles
                .Include(v => v.Photos)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<IEnumerable<Vehicle>> GetAllVehiclesAsync()
        {
            return await _context.Vehicles
                .Include(v => v.Photos)
                .ToListAsync();
        }

        public async Task<bool> UpdateVehicleAsync(Vehicle vehicle)
        {
            _context.Entry(vehicle).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteVehicleAsync(Guid id, string requestingUserId)
        {
            var vehicle = await _context.Vehicles
                .Include(v => v.Photos)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vehicle == null)
                return false;

            if (vehicle.OwnerId.ToString() != requestingUserId)
                return false;

            foreach (var photo in vehicle.Photos)
                _fileStorageService.DeleteVehicleImageAsync(photo.Url);

            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> VehicleExistsAsync(string licensePlate)
        {
            return await _context.Vehicles
                .AnyAsync(v => v.Specification.LicensePlate == licensePlate);
        }

        public async Task<IEnumerable<Vehicle>> GetVehiclesByOwnerIdAsync(string ownerId)
        {
            return await _context.Vehicles
                .Include(v => v.Photos)
                .Where(v => v.OwnerId.ToString() == ownerId)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Vehicles.AnyAsync(e => e.Id == id);
        }
    }
}
