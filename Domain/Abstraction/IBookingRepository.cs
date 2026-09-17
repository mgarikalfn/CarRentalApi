using Domain.Entities;

namespace Domain.Abstraction
{
    public interface IBookingRepository
    {
        Task<Booking?> GetByIdAsync(Guid id);
        Task<IEnumerable<Booking>> GetByUserIdAsync(Guid userId);
        Task<bool> IsVehicleBookedAsync(Guid vehicleId, DateTime startDate, DateTime endDate);
        Task AddAsync(Booking booking);
        Task UpdateAsync(Booking booking);
    }
}
