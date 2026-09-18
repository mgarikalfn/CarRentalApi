using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class Payment : AggregateRoot
{
    // ── Identity references (Guid only — no navigation properties) ──
    public Guid BookingId { get; private set; }
    public Guid PayerId { get; private set; }

    // ── Money value object ──
    public Money PaidAmount { get; private set; } = null!;

    // ── Payment method & state ──
    public PaymentMethod Method { get; private set; }
    public PaymentStatus Status { get; private set; }

    // ── Optional outcome fields ──
    public string? TransactionReference { get; private set; }
    public string? FailureReason { get; private set; }
    public string? RefundReason { get; private set; }

    // ── Timestamps ──
    public DateTime CreatedAt { get; private set; }
    public DateTime? SucceededAt { get; private set; }
    public DateTime? FailedAt { get; private set; }
    public DateTime? RefundedAt { get; private set; }

    // ── EF Core parameterless constructor ──
    private Payment() { }

    // ── Real constructor ──
    private Payment(
        Guid bookingId,
        Guid payerId,
        Money paidAmount,
        PaymentMethod method)
    {
        BookingId  = bookingId;
        PayerId    = payerId;
        PaidAmount = paidAmount;
        Method     = method;
        Status     = PaymentStatus.Pending;
        CreatedAt  = DateTime.UtcNow;

        AddDomainEvent(new PaymentCreatedEvent(Id, BookingId, PayerId));
    }

    // ── Factory ──
    public static Payment Create(
        Guid bookingId,
        Guid payerId,
        Money paidAmount,
        PaymentMethod method)
    {
        if (bookingId == Guid.Empty)
            throw new DomainException("BookingId is required.");

        if (payerId == Guid.Empty)
            throw new DomainException("PayerId is required.");

        if (paidAmount is null)
            throw new DomainException("PaidAmount is required.");

        EnumGuard.ValidateDefined(method, nameof(Method));

        return new Payment(bookingId, payerId, paidAmount, method);
    }

    // ── State transitions ──

    /// <summary>
    /// Marks a pending payment as succeeded. Guard: Status must be Pending.
    /// </summary>
    public void MarkSucceeded(string transactionReference)
    {
        // Guard reads this.Status — never a parameter
        if (Status != PaymentStatus.Pending)
            throw new DomainException("Only a pending payment can be marked as succeeded.");

        if (string.IsNullOrWhiteSpace(transactionReference))
            throw new DomainException("Transaction reference is required to mark a payment as succeeded.");

        TransactionReference = transactionReference.Trim();
        SucceededAt = DateTime.UtcNow;
        Status = PaymentStatus.Succeeded;

        AddDomainEvent(new PaymentSucceededEvent(Id, BookingId, PaidAmount));
    }

    /// <summary>
    /// Marks a pending payment as failed. Guard: Status must be Pending.
    /// </summary>
    public void MarkFailed(string reason)
    {
        // Guard reads this.Status — never a parameter
        if (Status != PaymentStatus.Pending)
            throw new DomainException("Only a pending payment can be marked as failed.");

        FailureReason = string.IsNullOrWhiteSpace(reason) ? "No reason provided." : reason.Trim();
        FailedAt = DateTime.UtcNow;
        Status = PaymentStatus.Failed;

        AddDomainEvent(new PaymentFailedEvent(Id, BookingId, FailureReason));
    }

    /// <summary>
    /// Refunds a succeeded payment. Guard: Status must be Succeeded.
    /// </summary>
    public void Refund(string reason)
    {
        // Guard reads this.Status — never a parameter
        if (Status != PaymentStatus.Succeeded)
            throw new DomainException("Only a succeeded payment can be refunded.");

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("A reason is required to refund a payment.");

        RefundReason = reason.Trim();
        RefundedAt = DateTime.UtcNow;
        Status = PaymentStatus.Refunded;

        AddDomainEvent(new PaymentRefundedEvent(Id, BookingId, PaidAmount));
    }
}
