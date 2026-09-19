using Application.Common;
using Application.Dto.DamageReport;
using MediatR;

namespace Application.Features.DamageReports.Queries;

public class GetDamageReportsByBookingQuery : IRequest<Result<List<DamageReportDto>>>
{
    public Guid BookingId { get; set; }
}
