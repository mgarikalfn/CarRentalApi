using Application.Common;
using Application.Dto.DamageReport;
using MediatR;

namespace Application.Features.DamageReports.Queries;

public class GetDamageReportByIdQuery : IRequest<Result<DamageReportDto>>
{
    public Guid DamageReportId { get; set; }
}
