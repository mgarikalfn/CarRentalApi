using Domain.Enums;

namespace Application.Dto.Payment;

/// <summary>
/// Inbound DTO for payment creation.
/// PayerId is NOT present — it is set by the controller from the authenticated
/// user's JWT identity. The client has no control over who the payer is.
/// </summary>
public class CreatePaymentRequest
{
    /// <summary>ID of the approved booking being paid for.</summary>
    public Guid BookingId { get; set; }

    /// <summary>
    /// Amount to pay. Must exactly match the booking's Price.Total.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>Payment method (Cash, Card, BankTransfer, MobileMoney).</summary>
    public PaymentMethod Method { get; set; }
}
