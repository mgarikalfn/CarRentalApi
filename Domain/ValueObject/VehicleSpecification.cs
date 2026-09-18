using System.Text.RegularExpressions;
using Domain.Common;

namespace Domain.Entities;

public record VehicleSpecification : ValueObject
{
    public string Brand { get; }
    public string Model { get; }
    public int Year { get; }
    public string Color { get; }
    public FuelType FuelType { get; }
    public TransmissionType Transmission { get; }
    public string Vin { get; }
    public string LicensePlate { get; }
    public int SeatCount { get; }

    public VehicleSpecification(
        string brand,
        string model,
        int year,
        string color,
        FuelType fuelType,
        TransmissionType transmission,
        string vin,
        string licensePlate,
        int seatCount)
    {
        Brand = ValidateBrand(brand);
        Model = ValidateModel(model);
        Year = ValidateYear(year);
        Color = ValidateColor(color);
        FuelType = EnumGuard.ValidateDefined(fuelType, nameof(FuelType));
        Transmission = EnumGuard.ValidateDefined(transmission, nameof(Transmission));
        Vin = ValidateVin(vin);
        LicensePlate = ValidateLicensePlate(licensePlate);
        SeatCount = ValidateSeatCount(seatCount);
    }

    private static string ValidateBrand(string brand)
    {
        if (string.IsNullOrWhiteSpace(brand))
            throw new DomainException("Brand is required.");

        brand = brand.Trim();
        if (brand.Length > 50)
            throw new DomainException("Brand cannot exceed 50 characters.");

        return brand;
    }

    private static string ValidateModel(string model)
    {
        if (string.IsNullOrWhiteSpace(model))
            throw new DomainException("Model is required.");

        model = model.Trim();
        if (model.Length > 50)
            throw new DomainException("Model cannot exceed 50 characters.");

        return model;
    }

    private static int ValidateYear(int year)
    {
        var currentYear = DateTime.UtcNow.Year;
        if (year < 1900)
            throw new DomainException("Invalid vehicle year.");
        if (year > currentYear + 1)
            throw new DomainException("Vehicle year cannot be in the future.");

        return year;
    }

    private static string ValidateColor(string color)
    {
        if (string.IsNullOrWhiteSpace(color))
            throw new DomainException("Color is required.");

        return color.Trim();
    }

    private static int ValidateSeatCount(int seats)
    {
        if (seats < 1)
            throw new DomainException("Seat count must be at least one.");
        if (seats > 100)
            throw new DomainException("Seat count is invalid.");

        return seats;
    }

    private static readonly Regex VinRegex = new("^[A-HJ-NPR-Z0-9]{17}$");

    private static string ValidateVin(string vin)
    {
        if (string.IsNullOrWhiteSpace(vin))
            throw new DomainException("VIN is required.");

        vin = vin.Trim().ToUpperInvariant();
        if (!VinRegex.IsMatch(vin))
            throw new DomainException("VIN format is invalid.");

        return vin;
    }

    private static string ValidateLicensePlate(string plate)
    {
        if (string.IsNullOrWhiteSpace(plate))
            throw new DomainException("License plate is required.");

        plate = plate.Trim();
        if (plate.Length > 15)
            throw new DomainException("License plate is too long.");

        return plate.ToUpperInvariant();
    }
}

public enum FuelType
{
    Petrol,
    Diesel,
    Electric,
    Hybrid
}

public enum TransmissionType
{
    Manual,
    Automatic
}

