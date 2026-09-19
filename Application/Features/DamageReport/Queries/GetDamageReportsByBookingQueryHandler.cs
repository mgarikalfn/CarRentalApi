using Application.Abstractions;
using Application.Common;
using Application.Dto.DamageReport;
using AutoMapper;
using MediatR;

namespace Application.Features.DamageReports.Queries;

public class GetDamageReportsByBookingQueryHandler
    : IRequestHandler<GetDamageReportsByBookingQuery, Result<List<DamageReportDto>>>
{
    private readonly IDamageReportRepository _repository;
    private readonly IMapper _mapper;

    public GetDamageReportsByBookingQueryHandler(IDamageReportRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper     = mapper;
    }

    public async Task<Result<List<DamageReportDto>>> Handle(
        GetDamageReportsByBookingQuery request,
        CancellationToken cancellationToken)
    {
        var reports = await _repository.GetByBookingIdAsync(request.BookingId, cancellationToken);
        if (!reports.Any())
            return Result<List<DamageReportDto>>.Failure("No damage reports found for this booking.");

        return Result<List<DamageReportDto>>.Success(_mapper.Map<List<DamageReportDto>>(reports));
    }
}
