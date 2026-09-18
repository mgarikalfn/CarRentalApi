using Application.Common;
using Domain.Entities;

namespace Application.Abstractions;

/// <summary>
/// Application-layer repository interface for Review.
/// Lives in Application (not Domain) because its AddAsync signature
/// declares DuplicateReviewException, which is an Application concern.
/// Infrastructure implements this interface.
/// </summary>
public interface IReviewRepository
{
    Task<Review?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Review>> GetByBookingIdAsync(Guid bookingId, CancellationToken ct = default);
    Task<IEnumerable<Review>> GetByRevieweeIdAsync(Guid revieweeId, CancellationToken ct = default);

    /// <summary>
    /// Persists a new Review.
    /// Throws <see cref="DuplicateReviewException"/> if a review from this reviewer
    /// for this booking already exists (unique constraint violation on
    /// UX_Reviews_BookingId_ReviewerId). Any other persistence failure propagates as-is.
    /// </summary>
    Task AddAsync(Review review, CancellationToken ct = default);

    Task UpdateAsync(Review review, CancellationToken ct = default);
}
