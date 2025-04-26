using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using FluentResults;

namespace Domain.Abstraction
{
    public interface IAvailabilityRepository
    {
        Task<int> CreateAvailabilityRepository(Availability availability);
        Task<Availability?> GetAvailabilityByIdAsync(int id);
        //Task<IEnumerable<Vehicle>> GetAllVehiclesAsync();
        //Task<Result<bool>> UpdateVehicleAsync(Vehicle vehicle);
        Task<bool> HasOverlappingAvailabilityAsync(int vehicleId, DateTime startDate,DateTime endDate, CancellationToken cancellationToken = default);
        Task<Result<bool>> DeleteAvailabilityAsync(int id, string requestingUserId,CancellationToken ct = default);
        //Task<bool> AvailabilityExistsAsync(string ownerId, int vehicleId);
       Task<Availability> GetAvailabilityByVehicleIdAsync(int vehicleId,int id,CancellationToken ct = default);
        Task<bool> ExistsAsync(int id);
    }
}
