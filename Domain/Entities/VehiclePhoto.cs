using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class VehiclePhoto : Entity
{
    public Guid VehicleId { get; private set; }


    public string Url { get; private set; } = string.Empty;


    public VehiclePhotoType Type { get; private set; }


    public bool IsPrimary { get; private set; }


    public DateTime UploadedAt { get; private set; }



    private VehiclePhoto()
    {

    }



    private VehiclePhoto(
        Guid vehicleId,
        string url,
        VehiclePhotoType type,
        bool isPrimary)
    {

        if(string.IsNullOrWhiteSpace(url))
        {
            throw new DomainException(
                "Photo url is required");
        }


        VehicleId = vehicleId;

        Url = url;

        Type = type;

        IsPrimary = isPrimary;

        UploadedAt = DateTime.UtcNow;
    }



    public static VehiclePhoto Create(
        Guid vehicleId,
        string url,
        VehiclePhotoType type,
        bool isPrimary = false)
    {
        return new VehiclePhoto(
            vehicleId,
            url,
            type,
            isPrimary);
    }



    public void SetAsPrimary()
    {
        IsPrimary = true;
    }
}