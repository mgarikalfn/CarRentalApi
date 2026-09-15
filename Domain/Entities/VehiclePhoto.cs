using Domain.Common;

namespace Domain.Entities;

public class VehiclePhoto : AuditableEntity
{
    public Guid VehicleId { get; private set; }
    public string Url { get; private set; } = string.Empty;
    public int DisplayOrder { get; private set; }

    private VehiclePhoto() { } // EF Core

    internal VehiclePhoto(Guid vehicleId, string url, int displayOrder)
    {
        if (vehicleId == Guid.Empty)
            throw new DomainException("VehicleId is required.");

        if (string.IsNullOrWhiteSpace(url))
            throw new DomainException("Photo URL is required.");

        VehicleId = vehicleId;
        Url = url.Trim();
        DisplayOrder = displayOrder;
    }
}