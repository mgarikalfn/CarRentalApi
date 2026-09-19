using Application.Common;
using Application.Dto.DamageReport;
using MediatR;

namespace Application.Features.DamageReports.Commands;

public class DismissCommand : IRequest<Result<DamageReportDto>>
{
    public Guid DamageReportId { get; set; }
}
