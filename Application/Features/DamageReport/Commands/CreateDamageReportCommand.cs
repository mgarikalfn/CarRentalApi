using Application.Common;
using Application.Dto.DamageReport;
using MediatR;

namespace Application.Features.DamageReports.Commands;

public class CreateDamageReportCommand : IRequest<Result<DamageReportDto>>
{
    public Guid BookingId { get; set; }
    public Guid ReportedByUserId { get; set; }   // server-set by controller from JWT
    public string Description { get; set; } = string.Empty;
    public List<string> ImageUrls { get; set; } = [];
}
