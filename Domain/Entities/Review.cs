using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class Review : AggregateRoot
{
    // ── Immutable fields ─────────────────────────────────────────────
    public Guid BookingId { get; private set; }
    public Guid VehicleId { get; private set; }
    public Guid ReviewerId { get; private set; }
    public Guid RevieweeId { get; private set; }
    public int Rating { get; private set; }
    public string? Comment { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // ── Mutable (admin/user-controlled moderation state) ─────────────
    public ReviewStatus Status { get; private set; }
    public bool IsFlaggedForReview { get; private set; }
    public DateTime? FlaggedAt { get; private set; }

    // ── EF Core constructor ──────────────────────────────────────────
    private Review() { }

    // ── Real constructor: validates, assigns, raises event ───────────
    private Review(
        Guid bookingId,
        Guid vehicleId,
        Guid reviewerId,
        Guid revieweeId,
        int rating,
        string? comment)
    {
        // Validate then assign — every field
        if (bookingId == Guid.Empty)
            throw new DomainException("BookingId is required.");
        BookingId = bookingId;

        if (vehicleId == Guid.Empty)
            throw new DomainException("VehicleId is required.");
        VehicleId = vehicleId;

        if (reviewerId == Guid.Empty)
            throw new DomainException("ReviewerId is required.");
        ReviewerId = reviewerId;

        if (revieweeId == Guid.Empty)
            throw new DomainException("RevieweeId is required.");
        RevieweeId = revieweeId;

        if (rating < 1 || rating > 5)
            throw new DomainException("Rating must be between 1 and 5.");
        Rating = rating;

        // Comment: optional; if provided, trim and enforce max 1000 chars
        if (comment is not null)
        {
            comment = comment.Trim();
            if (comment.Length > 1000)
                throw new DomainException("Comment cannot exceed 1000 characters.");
        }
        Comment = comment;

        Status = ReviewStatus.Published;
        IsFlaggedForReview = false;
        FlaggedAt = null;
        CreatedAt = DateTime.UtcNow;

        AddDomainEvent(new ReviewSubmittedEvent(Id, bookingId, revieweeId, vehicleId, rating));
    }

    // ── Factory: validates cross-field rules, then delegates ─────────
    public static Review Create(
        Guid bookingId,
        Guid vehicleId,
        Guid reviewerId,
        Guid revieweeId,
        int rating,
        string? comment)
    {
        if (reviewerId == revieweeId)
            throw new DomainException("A user cannot review themselves.");

        return new Review(bookingId, vehicleId, reviewerId, revieweeId, rating, comment);
    }

    // ── Moderation methods ───────────────────────────────────────────

    /// <summary>
    /// Any authenticated user may flag a review for moderation.
    /// Does NOT change Status — only sets IsFlaggedForReview = true.
    /// </summary>
    public void Flag()
    {
        if (IsFlaggedForReview)
            throw new DomainException("Review has already been flagged for moderation.");

        IsFlaggedForReview = true;
        FlaggedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Admin-only. Hides a published review from public view.
    /// </summary>
    public void Hide()
    {
        if (Status == ReviewStatus.Hidden)
            throw new DomainException("Review is already hidden.");

        Status = ReviewStatus.Hidden;
    }

    /// <summary>
    /// Admin-only. Restores a hidden or flagged review to published/clean state.
    /// Throws if the review is already fully restored (Published and not flagged).
    /// </summary>
    public void Restore()
    {
        // Hand-traced against all 4 combinations (Published/Hidden × flagged/not):
        //   (Published, not flagged) → both true  → throws ✅ nothing to do
        //   (Published, flagged)     → one false  → proceeds ✅ clears flag
        //   (Hidden, not flagged)    → one false  → proceeds ✅ restores
        //   (Hidden, flagged)        → both false → proceeds ✅ restores + clears flag
        if (Status == ReviewStatus.Published && !IsFlaggedForReview)
            throw new DomainException("Review is already published and not flagged.");

        Status = ReviewStatus.Published;
        IsFlaggedForReview = false;
        FlaggedAt = null;
    }
}
