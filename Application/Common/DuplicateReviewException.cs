namespace Application.Common;

/// <summary>
/// Thrown by IReviewRepository.AddAsync when a review from this reviewer
/// for this booking already exists (unique constraint violation on
/// UX_Reviews_BookingId_ReviewerId). Application catches this — it never
/// sees PostgresException or DbUpdateException directly.
/// </summary>
public sealed class DuplicateReviewException : Exception
{
    public DuplicateReviewException()
        : base("You have already submitted a review for this booking.") { }
}
