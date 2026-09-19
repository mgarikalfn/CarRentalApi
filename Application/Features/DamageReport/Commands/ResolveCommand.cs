using Application.Common;
using Application.Dto.DamageReport;
using MediatR;

namespace Application.Features.DamageReports.Commands;

public class ResolveCommand : IRequest<Result<DamageReportDto>>
{
    public Guid DamageReportId { get; set; }
    /// <summary>true = ResolvedAtFault, false = ResolvedNotAtFault</summary>
    public bool AtFault { get; set; }
}
