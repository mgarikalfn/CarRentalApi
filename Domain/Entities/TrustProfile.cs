using Domain.Enums;

namespace Domain.Entities;

public class TrustProfile : AuditableEntity
{
    public const decimal InitialScore = 50m;

    public string UserId { get; private set; } = string.Empty;
    public ApplicationUser User { get; private set; } = null!;

    public decimal OverallScore { get; private set; }
        = InitialScore;

    public VerificationLevel VerificationLevel
        { get; private set; }
        = VerificationLevel.Unverified;

    public int CompletedRentals { get; private set; }
    public int CancelledBookings { get; private set; }
    public int LateReturns { get; private set; }

    public int DamageClaimsTotal { get; private set; }
    public int DamageClaimsAtFault { get; private set; }

    public decimal? AverageRatingAsRenter
        { get; private set; }

    public decimal? AverageRatingAsHost
        { get; private set; }

    public decimal? ResponseRate
        { get; private set; }

    public int? AvgResponseTimeMinutes
        { get; private set; }

    public DateTime LastRecalculatedAt
        { get; private set; }

    public bool IsUnderReview
        { get; private set; }

    public string? ReviewReason
        { get; private set; }

    public DateTime? FlaggedAt
        { get; private set; }

    public decimal CancellationRate =>
        CompletedRentals + CancelledBookings == 0
            ? 0
            : (decimal)CancelledBookings /
              (CompletedRentals + CancelledBookings);

    public decimal DamageAtFaultRate =>
        DamageClaimsTotal == 0
            ? 0
            : (decimal)DamageClaimsAtFault /
              DamageClaimsTotal;

    private TrustProfile()
    {
    }

    public void Recalculate(
        decimal score,
        VerificationLevel verificationLevel)
    {
        OverallScore = score;
        VerificationLevel = verificationLevel;
        LastRecalculatedAt = DateTime.UtcNow;
    }

    public void Flag(string reason)
    {
        IsUnderReview = true;
        ReviewReason = reason;
        FlaggedAt = DateTime.UtcNow;
    }

    public void ClearFlag()
    {
        IsUnderReview = false;
        ReviewReason = null;
        FlaggedAt = null;
    }
}