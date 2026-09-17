using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Abstraction
{
    public interface IBookingRepository
    {
        Task<Booking?> GetByIdAsync(Guid id);
        Task<Booking?> GetByIdWithVehicleAsync(int id);
        Task<IEnumerable<Booking>> GetByUserIdAsync(string userId);
        Task<bool> IsVehicleBookedAsync(Guid vehicleId, DateTime startDate, DateTime endDate);
        Task AddAsync(Booking booking);
        Task UpdateAsync(Booking booking);
    }
}
