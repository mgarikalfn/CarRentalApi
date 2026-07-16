using System.Text.RegularExpressions;
using Domain.Common;

namespace Domain.Entities;

public record VehicleSpecification : ValueObject
{
     public string Brand { get; private set; } = string.Empty;
    public string Model { get; private set; } = string.Empty;
    public int Year { get; private set; }

    public string Color { get; private set; } = string.Empty;

    public FuelType FuelType { get; private set; }

    public TransmissionType Transmission { get; private set; }
    public string VIN{get; private set;} = string.Empty;
    public string LicensePlate {get; private set;}=string.Empty;             
    public int SeatCount { get; private set; }

    internal VehicleSpecification(string brand,string model , int year,string color , FuelType fuelType, TransmissionType transmissionType, string vin , string licensePlate, int seatCount)
    {
          Brand = ValidateBrand(brand);
    Model = ValidateModel(model);
    Year = ValidateYear(year);
    Color = ValidateColor(color);
    FuelType = EnumGuard.ValidateDefined(fuelType,nameof(FuelType));
    Transmission = EnumGuard.ValidateDefined(transmissionType,nameof(TransmissionType));
    VIN = ValidateVin(vin);
    LicensePlate = ValidateLicensePlate(licensePlate);
    SeatCount = ValidateSeatCount(seatCount);
    }

    private static string ValidateBrand(string brand)
    {
        brand=brand.Trim();
        if (string.IsNullOrWhiteSpace(brand))
        {
            throw new InvalidOperationException("Brand is required");
        }
        if(brand.Length > 50)
        {
            throw new InvalidOperationException("brand cannot exceed 50 characters.");
        }
        return brand;
    }

    private static string ValidateModel(string model)
{
    model = model.Trim();
    if (string.IsNullOrWhiteSpace(model))
        throw new InvalidOperationException("Model is required.");

    if (model.Length > 50)
        throw new InvalidOperationException("Model cannot exceed 50 characters.");

    return model;
}

private static int ValidateYear(int year)
{
    var currentYear = DateTime.UtcNow.Year;

    if (year < 1990)
        throw new InvalidOperationException("Invalid vehicle year.");

    if (year > currentYear + 1)
        throw new InvalidOperationException("Vehicle year cannot be in the distant future.");

    return year;
}

private static string ValidateColor(string color)
{
    if (string.IsNullOrWhiteSpace(color))
        throw new InvalidOperationException("Color is required.");

    return color.Trim();
}

private static int ValidateSeatCount(int seats)
{
    if (seats < 1)
        throw new InvalidOperationException("Seat count must be at least one.");

    if (seats > 100)
        throw new InvalidOperationException("Seat count is invalid.");

    return seats;
}

private static readonly Regex VinRegex =
    new("^[A-HJ-NPR-Z0-9]{17}$");

private static string ValidateVin(string vin)
{
    if (string.IsNullOrWhiteSpace(vin))
        throw new InvalidOperationException("VIN is required.");

    vin = vin.Trim().ToUpperInvariant();

    if (!VinRegex.IsMatch(vin))
        throw new InvalidOperationException("VIN format is invalid.");

    return vin;
}

private static readonly Regex PlateRegex =
    new("^[A-Za-z0-9- ]{3,15}$");

private static string ValidateLicensePlate(string plate)
{
    if (string.IsNullOrWhiteSpace(plate))
        throw new InvalidOperationException("License plate is required.");

    plate = plate.Trim().ToUpperInvariant();

    if (!PlateRegex.IsMatch(plate))
        throw new InvalidOperationException("License plate format is invalid.");

    return plate;
}

}

