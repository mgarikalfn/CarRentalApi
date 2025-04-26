using Domain.Entities;
using Domain.Common;


namespace Domain.Abstraction
{
    public interface IVehicleRepository
    {
        Task<int> CreateVehicleAsync(Vehicle vehicle);
        Task<Vehicle?> GetVehicleByIdAsync(int id);
        Task<IEnumerable<Vehicle>> GetAllVehiclesAsync();
        Task<Result<bool>> UpdateVehicleAsync(Vehicle vehicle);
        Task<Result<bool>> DeleteVehicleAsync(int id, string requestingUserId);
        Task<bool> VehicleExistsAsync(string licensePlate);
        Task<IEnumerable<Vehicle>> GetVehiclesByOwnerIdAsync(string ownerId);
        Task<bool> ExistsAsync(int id);
    }
}
