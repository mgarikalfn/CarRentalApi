using Application.Abstractions;
using Application.Common;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Infrastructure.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly RentalDbContext _context;

    public ReviewRepository(RentalDbContext context) => _context = context;

    public async Task<Review?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Reviews.FirstOrDefaultAsync(r => r.Id == id, ct);

    public async Task<IEnumerable<Review>> GetByBookingIdAsync(Guid bookingId, CancellationToken ct = default)
        => await _context.Reviews
            .Where(r => r.BookingId == bookingId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(ct);

    public async Task<IEnumerable<Review>> GetByRevieweeIdAsync(Guid revieweeId, CancellationToken ct = default)
        => await _context.Reviews
            .Where(r => r.RevieweeId == revieweeId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(ct);

    public async Task AddAsync(Review review, CancellationToken ct = default)
    {
        _context.Reviews.Add(review);
        try
        {
            await _context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is PostgresException pg
                  && pg.SqlState == "23505"
                  && pg.ConstraintName == "UX_Reviews_BookingId_ReviewerId")
        {
            throw new DuplicateReviewException();
        }
        // Any other DbUpdateException propagates — genuine infrastructure error.
    }

    public async Task UpdateAsync(Review review, CancellationToken ct = default)
    {
        _context.Reviews.Update(review);
        await _context.SaveChangesAsync(ct);
    }
}
