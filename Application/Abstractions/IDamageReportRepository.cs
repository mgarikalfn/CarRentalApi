using Domain.Entities;

namespace Application.Abstractions;

public interface IDamageReportRepository
{
    Task<DamageReport?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<DamageReport>> GetByBookingIdAsync(Guid bookingId, CancellationToken ct = default);
    Task AddAsync(DamageReport report, CancellationToken ct = default);
    Task UpdateAsync(DamageReport report, CancellationToken ct = default);
}
