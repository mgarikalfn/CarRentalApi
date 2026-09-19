using Application.Abstractions;
using Application.Common;
using Application.Dto.DamageReport;
using AutoMapper;
using Domain.Common;
using MediatR;

namespace Application.Features.DamageReports.Commands;

public class StartReviewCommandHandler
    : IRequestHandler<StartReviewCommand, Result<DamageReportDto>>
{
    private readonly IDamageReportRepository _repository;
    private readonly IMapper _mapper;

    public StartReviewCommandHandler(IDamageReportRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper     = mapper;
    }

    public async Task<Result<DamageReportDto>> Handle(
        StartReviewCommand request,
        CancellationToken cancellationToken)
    {
        var report = await _repository.GetByIdAsync(request.DamageReportId, cancellationToken);
        if (report is null)
            return Result<DamageReportDto>.Failure("Damage report not found.");

        try { report.StartReview(); }
        catch (DomainException ex) { return Result<DamageReportDto>.Failure(ex.Message); }

        await _repository.UpdateAsync(report, cancellationToken);
        return Result<DamageReportDto>.Success(_mapper.Map<DamageReportDto>(report));
    }
}
