using Application.Abstractions;
using Application.Common;
using Application.Dto.DamageReport;
using AutoMapper;
using Domain.Abstraction;
using Domain.Entities;
using Domain.Common;
using Domain.Enums;
using MediatR;

namespace Application.Features.DamageReports.Commands;

public class CreateDamageReportCommandHandler
    : IRequestHandler<CreateDamageReportCommand, Result<DamageReportDto>>
{
    private readonly IDamageReportRepository _reportRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMapper _mapper;

    public CreateDamageReportCommandHandler(
        IDamageReportRepository reportRepository,
        IBookingRepository bookingRepository,
        IVehicleRepository vehicleRepository,
        IMapper mapper)
    {
        _reportRepository  = reportRepository;
        _bookingRepository = bookingRepository;
        _vehicleRepository = vehicleRepository;
        _mapper            = mapper;
    }

    public async Task<Result<DamageReportDto>> Handle(
        CreateDamageReportCommand request,
        CancellationToken cancellationToken)
    {
        // Cross-aggregate check: booking must exist and be completed/active
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId);
        if (booking is null)
            return Result<DamageReportDto>.Failure("Booking not found.");

        if (booking.Status != BookingStatus.Completed && booking.Status != BookingStatus.Active)
            return Result<DamageReportDto>.Failure(
                "Damage reports can only be filed for active or completed bookings.");

        // Load vehicle to confirm reporter is a party and get VehicleId
        var vehicle = await _vehicleRepository.GetVehicleByIdAsync(booking.VehicleId);
        if (vehicle is null)
            return Result<DamageReportDto>.Failure("Vehicle not found.");

        var hostId   = vehicle.OwnerId;
        var renterId = booking.RenterId;

        if (request.ReportedByUserId != renterId && request.ReportedByUserId != hostId)
            return Result<DamageReportDto>.Failure("Only a party to the booking can file a damage report.");

        var report = DamageReport.Create(
            booking.Id,
            vehicle.Id,
            request.ReportedByUserId,
            request.Description);

        foreach (var url in request.ImageUrls)
        {
            try { report.AddImage(url); }
            catch (DomainException ex) { return Result<DamageReportDto>.Failure(ex.Message); }
        }

        await _reportRepository.AddAsync(report, cancellationToken);
        return Result<DamageReportDto>.Success(_mapper.Map<DamageReportDto>(report));
    }
}
