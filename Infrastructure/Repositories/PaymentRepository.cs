using Application.Abstractions;
using Application.Common;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly RentalDbContext _context;

    public PaymentRepository(RentalDbContext context)
    {
        _context = context;
    }

    public async Task<Payment?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Payments.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<IEnumerable<Payment>> GetByBookingIdAsync(Guid bookingId, CancellationToken ct = default)
        => await _context.Payments
            .Where(p => p.BookingId == bookingId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(ct);

    public async Task AddAsync(Payment payment, CancellationToken ct = default)
    {
        _context.Payments.Add(payment);
        try
        {
            await _context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is PostgresException pg
                  && pg.SqlState == "23505"
                  && pg.ConstraintName == "UX_Payments_BookingId_Active")
        {
            throw new DuplicateActivePaymentException();
        }
        // Any other DbUpdateException propagates — genuine infrastructure error.
    }

    public async Task UpdateAsync(Payment payment, CancellationToken ct = default)
    {
        _context.Payments.Update(payment);
        await _context.SaveChangesAsync(ct);
    }
}
