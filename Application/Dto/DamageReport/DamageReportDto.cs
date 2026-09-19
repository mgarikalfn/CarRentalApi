using Domain.Enums;

namespace Application.Dto.DamageReport;

public class DamageReportDto
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public Guid VehicleId { get; set; }
    public Guid ReportedByUserId { get; set; }
    public string Description { get; set; } = string.Empty;
    public DamageReportStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public IReadOnlyCollection<DamageImageDto> Images { get; set; } = [];
}

public class DamageImageDto
{
    public string Url { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
}
