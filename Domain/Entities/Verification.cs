using Domain.Enums;

namespace Domain.Entities;

public class VerificationRecord : AuditableEntity
{
    // FK to ApplicationUser (the renter/host whose identity is being verified)
    // Guid to match ApplicationUser.Id — no shadow property workaround
    public Guid UserId { get; private set; }
    public ApplicationUser User { get; private set; } = null!;

    public VerificationType Type { get; private set; }

    public VerificationStatus Status { get; private set; }

    public string? DocumentNumber { get; private set; }

    public string? DocumentUrl { get; private set; }

    public string? RejectionReason { get; private set; }

    public DateTime? VerifiedAt { get; private set; }

    // FK to the admin/staff user who reviewed this record.
    // Nullable — null means not yet reviewed.
    // Guid? to match ApplicationUser.Id — not an opaque external identifier.
    public Guid? VerifiedByUserId { get; private set; }
}
