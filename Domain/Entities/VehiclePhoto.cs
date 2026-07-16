namespace Domain.Entities;

public class VehiclePhoto : AuditableEntity
{
    public Guid VehicleId { get; private set;}
    public string URL {get; private set;} = String.Empty;
    public int? DisplayOrder { get; private set;} 

    internal VehiclePhoto(Guid vehicleId,string url, int? displayOrder)
    {
        VehicleId = vehicleId;
        URL = url;
        DisplayOrder = displayOrder;
    }
}