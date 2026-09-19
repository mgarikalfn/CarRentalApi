using Application.Abstractions;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class DamageReportRepository : IDamageReportRepository
{
    private readonly RentalDbContext _context;

    public DamageReportRepository(RentalDbContext context) => _context = context;

    public async Task<DamageReport?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.DamageReports
            .Include("Images")
            .FirstOrDefaultAsync(d => d.Id == id, ct);

    public async Task<IEnumerable<DamageReport>> GetByBookingIdAsync(
        Guid bookingId, CancellationToken ct = default)
        => await _context.DamageReports
            .Include("Images")
            .Where(d => d.BookingId == bookingId)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync(ct);

    public async Task AddAsync(DamageReport report, CancellationToken ct = default)
    {
        _context.DamageReports.Add(report);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(DamageReport report, CancellationToken ct = default)
    {
        _context.DamageReports.Update(report);
        await _context.SaveChangesAsync(ct);
    }
}
