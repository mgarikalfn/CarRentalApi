using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public VehicleRepository(RentalDbContext context,IFileStorageService fileStorageService)
        {
            _context = context;
            _fileStorageService = fileStorageService;
        }
        public async  Task<int> CreateVehicleAsync(Vehicle vehicle)
        {
           await _context.Vehicles.AddAsync(vehicle);
            await _context.SaveChangesAsync();  
            return vehicle.Id;
        }

        public async Task<Result<bool>> DeleteVehicleAsync(int id, string requestingUserId)
        {
            // 1. Find vehicle
            var vehicle = await _context.Vehicles
                .Include(v => v.Images)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vehicle == null)
            {
                return Domain.Common.Result<bool>.Failure("Vehicle not found", "NOT_FOUND");
            }

            // 2. Verify ownership
            if (vehicle.OwnerId != requestingUserId)
            {
                return Domain.Common.Result<bool>.Failure("Unauthorized deletion attempt", "UNAUTHORIZED");
            }

            // 3. Delete associated images
            if (vehicle.Images != null)
            {
                foreach (var image in vehicle.Images)
                {
                     _fileStorageService.DeleteVehicleImageAsync(image.ImageUrl);
                }
            }

            // 4. Remove vehicle
            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();

            return Domain.Common.Result<bool>.Success(true);
        }

        public Task<IEnumerable<Vehicle>> GetAllVehiclesAsync()
        {
            throw new NotImplementedException();
        }

        public async  Task<Vehicle?> GetVehicleByIdAsync(int id)
        {
            return await _context.Vehicles
                .Include(v => v.Images)
                 .FirstOrDefaultAsync(v => v.Id == id);
        }

        public Task<IEnumerable<Vehicle>> GetVehiclesByOwnerIdAsync(string ownerId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<bool>> UpdateVehicleAsync(Vehicle vehicle)
        {
            _context.Entry(vehicle).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Result<bool>.Success(true);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!await ExistsAsync(vehicle.Id))
                {
                    return Result<bool>.Failure("Vehicle not found", "NOT_FOUND");
                }

                // For concurrency conflicts, return specific error
                return Result<bool>.Failure(
                    "Vehicle was modified by another user. Please refresh and try again.",
                    "CONCURRENCY_CONFLICT");
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Update failed: {ex.Message}", "UPDATE_FAILED");
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Vehicles.AnyAsync(e => e.Id == id);
        }

        public async Task<bool> VehicleExistsAsync(string licensePlate)
        {
            return await _context.Vehicles.AnyAsync(v => v.LicensePlate == licensePlate);
        }

      

      
    }
}
