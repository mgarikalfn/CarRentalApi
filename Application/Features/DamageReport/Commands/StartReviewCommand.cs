using Application.Common;
using Application.Dto.DamageReport;
using MediatR;

namespace Application.Features.DamageReports.Commands;

public class StartReviewCommand : IRequest<Result<DamageReportDto>>
{
    public Guid DamageReportId { get; set; }
}
