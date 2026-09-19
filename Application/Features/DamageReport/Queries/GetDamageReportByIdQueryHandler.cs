using Application.Abstractions;
using Application.Common;
using Application.Dto.DamageReport;
using AutoMapper;
using MediatR;

namespace Application.Features.DamageReports.Queries;

public class GetDamageReportByIdQueryHandler
    : IRequestHandler<GetDamageReportByIdQuery, Result<DamageReportDto>>
{
    private readonly IDamageReportRepository _repository;
    private readonly IMapper _mapper;

    public GetDamageReportByIdQueryHandler(IDamageReportRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper     = mapper;
    }

    public async Task<Result<DamageReportDto>> Handle(
        GetDamageReportByIdQuery request,
        CancellationToken cancellationToken)
    {
        var report = await _repository.GetByIdAsync(request.DamageReportId, cancellationToken);
        if (report is null)
            return Result<DamageReportDto>.Failure("Damage report not found.");

        return Result<DamageReportDto>.Success(_mapper.Map<DamageReportDto>(report));
    }
}
