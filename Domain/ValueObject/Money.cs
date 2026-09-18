using Domain.Common;

namespace Domain.Entities;

public record Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency = "BIRR")
    {
        if (amount <= 0)
            throw new DomainException("Amount must be positive.");

        if (string.IsNullOrWhiteSpace(currency))
            throw new DomainException("Currency is required.");

        Amount = amount;
        Currency = currency.Trim().ToUpperInvariant();
    }
}
