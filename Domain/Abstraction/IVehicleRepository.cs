using Domain.Entities;
using Domain.Common;

namespace Domain.Abstraction
{
    public interface IVehicleRepository
    {
        Task<int> CreateVehicleAsync(Vehicle vehicle);
        Task<Vehicle?> GetVehicleByIdAsync(Guid id);
        Task<IEnumerable<Vehicle>> GetAllVehiclesAsync();
        Task<bool> UpdateVehicleAsync(Vehicle vehicle);
        Task<bool> DeleteVehicleAsync(Guid id, string requestingUserId);
        Task<bool> VehicleExistsAsync(string licensePlate);
        Task<IEnumerable<Vehicle>> GetVehiclesByOwnerIdAsync(string ownerId);
        Task<bool> ExistsAsync(Guid id);
    }
}
