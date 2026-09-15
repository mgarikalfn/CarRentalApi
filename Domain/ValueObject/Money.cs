using Domain.Common;

namespace Domain.Entities;

public record Money : ValueObject
{
    public decimal DailyPrice { get; }
    public string Currency { get; }

    public Money(decimal dailyPrice, string currency = "BIRR")
    {
        if (dailyPrice <= 0)
            throw new DomainException("Daily price must be positive.");

        if (string.IsNullOrWhiteSpace(currency))
            throw new DomainException("Currency is required.");

        DailyPrice = dailyPrice;
        Currency = currency.Trim().ToUpperInvariant();
    }
}