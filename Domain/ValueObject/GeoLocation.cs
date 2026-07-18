using Domain.Common;

namespace Domain.Entities;

public record GeoLocation : ValueObject
{
    public double Latitude{get; private set;}
    public double Longitude {get; private set;}
    public string City {get; private set;} = String.Empty;

    public GeoLocation(double latitude,double longitude, string city)
    {
        Latitude = ValidateLatitude(latitude);
        Longitude = ValidateLongitude(longitude);
        City = ValidateCity(city);
    }

    private static double ValidateLatitude(double latitude)
    {
        if(latitude < -90 || latitude > 90)
        {
            throw new DomainException($"Latitude must be between -90 and 90 . Received:{latitude}");
        }
        return latitude;
    }
    private static double ValidateLongitude(double longitude)
    {
        if(longitude < -180 || longitude > 180)
        {
            throw new DomainException($"longitude must be between -180 and 180 . Received:{longitude}");
        }
        return longitude;
    }

    private static string ValidateCity(string city)
    {
        city = city.Trim();
        if (string.IsNullOrWhiteSpace(city))
        {
            throw new DomainException("city is required");
        }
        if(city.Length > 100)
        {
            throw new DomainException("city name can't exceed 100 characters");
        }
        return city;
    }
}