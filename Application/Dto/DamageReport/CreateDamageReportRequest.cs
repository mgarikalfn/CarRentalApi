namespace Application.Dto.DamageReport;

/// <summary>
/// Inbound DTO — contains only fields the client legitimately controls.
/// ReportedByUserId is NOT here: it is set by the controller from JWT identity.
/// </summary>
public class CreateDamageReportRequest
{
    public Guid BookingId { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<string> ImageUrls { get; set; } = [];
}
