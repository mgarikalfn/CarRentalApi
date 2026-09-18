using Application.Dto.Availablity;
using AutoMapper;
using Domain.Abstraction;
using FluentResults;
using MediatR;

namespace Application.Features.Availabilities.Query
{
    public class GetVehicleByIdHandler : IRequestHandler<GetAvailabilityById, Result<List<AvailabilityDto>>>
    {
        private readonly IAvailabilityRepository _availabilityRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IMapper _mapper;

        public GetVehicleByIdHandler(
            IAvailabilityRepository availabilityRepository,
            IVehicleRepository vehicleRepository,
            IMapper mapper)
        {
            _availabilityRepository = availabilityRepository;
            _vehicleRepository = vehicleRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<AvailabilityDto>>> Handle(GetAvailabilityById request, CancellationToken cancellationToken)
        {
            var vehicleExists = await _vehicleRepository.ExistsAsync(request.VehicleId);
            if (!vehicleExists)
                return Result.Fail<List<AvailabilityDto>>("Vehicle not found");

            // GetAvailabilityByVehicleIdAsync(int id, Guid vehicleId) — id=0 means "find any for this vehicle"
            var avail = await _availabilityRepository.GetAvailabilityByVehicleIdAsync(0, request.VehicleId, cancellationToken);
            var availabilities = avail != null
                ? new List<Domain.Entities.Availability> { avail }
                : new List<Domain.Entities.Availability>();

            if (!availabilities.Any())
                return Result.Fail<List<AvailabilityDto>>("No availabilities found");

            var availabilityDtos = _mapper.Map<List<AvailabilityDto>>(availabilities);
            return Result.Ok(availabilityDtos);
        }
    }
}
