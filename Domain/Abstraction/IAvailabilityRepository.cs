using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;
using FluentResults;

namespace Domain.Abstraction
{
    public interface IAvailabilityRepository
    {
        Task<int> CreateAvailabilityRepository(Availability availability);
        Task<Availability?> GetAvailabilityByIdAsync(int id);
        Task<bool> HasOverlappingAvailabilityAsync(Guid vehicleId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        Task<Result<bool>> DeleteAvailabilityAsync(int id, string requestingUserId, CancellationToken ct = default);
        Task<Availability?> GetAvailabilityByVehicleIdAsync(int id, Guid vehicleId, CancellationToken ct = default);
        Task<bool> ExistsAsync(int id);
    }
}
