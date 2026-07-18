using Domain.Common;

namespace Domain.Entities;

public record Money : ValueObject
{
    public decimal DailyPrice {get; private set;}
    public string Currency {get; private set;} 

    internal Money( decimal dailyPrice, string currency = "BIRR")
    {
        if(dailyPrice <= 0)
        {
            throw new ArgumentException("price must be a positive value");
        }
         if (string.IsNullOrWhiteSpace(currency))
        {
            throw new ArgumentException("Currency is required.");
        }
        DailyPrice = dailyPrice;
        Currency = currency; 

    }
}