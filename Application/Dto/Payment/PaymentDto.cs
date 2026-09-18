using Domain.Enums;

namespace Application.Dto.Payment;

public class PaymentDto
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public Guid PayerId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "BIRR";
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; }
    public string? TransactionReference { get; set; }
    public string? FailureReason { get; set; }
    public string? RefundReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? SucceededAt { get; set; }
    public DateTime? FailedAt { get; set; }
    public DateTime? RefundedAt { get; set; }
}
