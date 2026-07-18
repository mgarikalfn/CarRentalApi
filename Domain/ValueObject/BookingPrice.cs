using Domain.Common;

namespace Domain.Entities;

public record BookingPrice : ValueObject
{
    public decimal Subtotal { get; }
    public decimal Discount { get; }
    public decimal InsuranceCost { get; }
    public decimal ServiceFee { get; }
    public decimal TaxRate { get; }      // e.g. 0.15 = 15%, not a currency amount
    public string Currency { get; }

    public BookingPrice(
        decimal subtotal,
        decimal discount,
        decimal insuranceCost,
        decimal serviceFee,
        decimal taxRate,
        string currency = "BIRR")
    {
        if (subtotal < 0)
            throw new DomainException("Subtotal cannot be negative.");

        if (discount < 0 || discount > subtotal)
            throw new DomainException("Discount cannot be negative or exceed subtotal.");

        if (insuranceCost < 0)
            throw new DomainException("Insurance cost cannot be negative.");

        if (serviceFee < 0)
            throw new DomainException("Service fee cannot be negative.");

        if (taxRate < 0 || taxRate > 1)
            throw new DomainException("Tax rate must be between 0 and 1, inclusive.");

        if (string.IsNullOrWhiteSpace(currency))
            throw new DomainException("Currency is required.");

        // <-- this assignment step was missing entirely in your version;
        // every field silently stayed at its default (0m) forever
        Subtotal = subtotal;
        Discount = discount;
        InsuranceCost = insuranceCost;
        ServiceFee = serviceFee;
        TaxRate = taxRate;
        Currency = currency;
    }

    // Derived, never stored — cannot ever drift out of sync with its inputs
    public decimal DiscountedSubtotal => Subtotal - Discount;
    public decimal TaxAmount => DiscountedSubtotal * TaxRate;
    public decimal Total => DiscountedSubtotal + InsuranceCost + ServiceFee + TaxAmount;
}