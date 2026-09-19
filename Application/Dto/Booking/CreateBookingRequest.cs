namespace Application.Dto.Booking;

/// <summary>
/// Inbound DTO for booking creation.
/// RenterId is NOT present — it is set by the controller from the authenticated
/// user's JWT identity. The client has no control over who the renter is.
/// </summary>
public class CreateBookingRequest
{
    /// <summary>ID of the vehicle to book.</summary>
    public Guid VehicleId { get; set; }

    /// <summary>Requested start date (must be in the future).</summary>
    public DateTime StartDate { get; set; }

    /// <summary>Requested end date (must be after StartDate).</summary>
    public DateTime EndDate { get; set; }

    /// <summary>Physical pick-up location description.</summary>
    public string PickUpLocation { get; set; } = string.Empty;

    /// <summary>Physical drop-off location description.</summary>
    public string DropOffLocation { get; set; } = string.Empty;
}
