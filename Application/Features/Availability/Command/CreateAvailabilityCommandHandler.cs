using AutoMapper;
using Application.Common;
using Domain.Abstraction;
using Domain.Enums;
using MediatR;

namespace Application.Features.Availability.Command
{
    public class CreateAvailabilityCommandHandler : IRequestHandler<CreateAvailabilityCommand, Result<int>>
    {
        private readonly IAvailabilityRepository _availabilityRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IMapper _mapper;

        public CreateAvailabilityCommandHandler(IVehicleRepository vehicleRepository, IAvailabilityRepository availabilityRepository, IMapper mapper)
        {
            _vehicleRepository = vehicleRepository;
            _availabilityRepository = availabilityRepository;
            _mapper = mapper;
        }

        public async Task<Result<int>> Handle(CreateAvailabilityCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
                return Result<int>.Failure("Request cannot be null");

            var vehicle = await _vehicleRepository.GetVehicleByIdAsync(request.VehicleId);
            if (vehicle == null)
                return Result<int>.Failure("Vehicle doesn't exist");

            if (vehicle.OwnerId != request.OwnerId)
                return Result<int>.Failure("Unauthorized access");

            if (request.EndDate <= request.StartDate)
                return Result<int>.Failure("End date must be after start date");

            var isOverlapping = await _availabilityRepository.HasOverlappingAvailabilityAsync(
                vehicle.Id, request.StartDate, request.EndDate, cancellationToken);

            if (isOverlapping)
                return Result<int>.Failure("Overlapping availability exists");

            var availability = Domain.Entities.Availability.Create(
                vehicle.Id,
                request.StartDate,
                request.EndDate,
                request.Type);

            var result = await _availabilityRepository.CreateAvailabilityRepository(availability);
            return Result<int>.Success(result);
        }
    }
}
