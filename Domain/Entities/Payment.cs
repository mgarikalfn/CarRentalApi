using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class Payment : AggregateRoot
{
    public Guid BookingId { get; private set; }

    public Guid PayerId { get; private set; }


    public decimal Amount { get; private set; }

    public string Currency { get; private set; } = "USD";


    public PaymentMethod Method { get; private set; }

    public PaymentStatus Status { get; private set; }


    public string? TransactionReference { get; private set; }


    public DateTime? CompletedAt { get; private set; }

    public DateTime? RefundedAt { get; private set; }


    private Payment()
    {

    }


    private Payment(
        Guid bookingId,
        Guid payerId,
        decimal amount,
        string currency,
        PaymentMethod method)
    {

        if(amount <= 0)
            throw new DomainException(
                "Payment amount must be positive");


        BookingId = bookingId;

        PayerId = payerId;

        Amount = amount;

        Currency = currency;

        Method = method;

        Status = PaymentStatus.Pending;
    }


    public static Payment Create(
        Guid bookingId,
        Guid payerId,
        decimal amount,
        string currency,
        PaymentMethod method)
    {
        return new Payment(
            bookingId,
            payerId,
            amount,
            currency,
            method);
    }


    public void MarkProcessing()
    {
        if(Status != PaymentStatus.Pending)
            throw new DomainException(
                "Only pending payments can be processed");


        Status = PaymentStatus.Processing;
    }


    public void Complete(string transactionReference)
    {
        if(Status != PaymentStatus.Processing)
            throw new DomainException(
                "Payment is not processing");


        TransactionReference = transactionReference;

        Status = PaymentStatus.Completed;

        CompletedAt = DateTime.UtcNow;
    }


    public void Fail()
    {
        if(Status == PaymentStatus.Completed)
            throw new DomainException(
                "Completed payment cannot fail");


        Status = PaymentStatus.Failed;
    }


    public void Refund()
    {
        if(Status != PaymentStatus.Completed)
            throw new DomainException(
                "Only completed payments can be refunded");


        Status = PaymentStatus.Refunded;

        RefundedAt = DateTime.UtcNow;
    }
}