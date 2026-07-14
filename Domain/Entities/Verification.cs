using Domain.Enums;

namespace Domain.Entities;

public class VerificationRecord : AuditableEntity
{
    public string UserId { get; private set; } = string.Empty;
    public ApplicationUser User { get; private set; } = null!;

    public VerificationType Type { get; private set; }

    public VerificationStatus Status { get; private set; }

    public string? DocumentNumber { get; private set; }

    public string? DocumentUrl { get; private set; }

    public string? RejectionReason { get; private set; }

    public DateTime? VerifiedAt { get; private set; }

    public string? VerifiedByUserId { get; private set; }
}

