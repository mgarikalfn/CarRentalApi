using Application.Common;
using Application.Dto.Availablity;
using AutoMapper;
using Domain.Abstraction;
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

        public async Task<Result<List<AvailabilityDto>>> Handle(
            GetAvailabilityById request,
            CancellationToken cancellationToken)
        {
            var vehicleExists = await _vehicleRepository.ExistsAsync(request.VehicleId);
            if (!vehicleExists)
                return Result<List<AvailabilityDto>>.Failure("Vehicle not found");

            var avail = await _availabilityRepository.GetAvailabilityByVehicleIdAsync(
                0, request.VehicleId, cancellationToken);

            var availabilities = avail != null
                ? new List<Domain.Entities.Availability> { avail }
                : new List<Domain.Entities.Availability>();

            if (!availabilities.Any())
                return Result<List<AvailabilityDto>>.Failure("No availabilities found");

            return Result<List<AvailabilityDto>>.Success(_mapper.Map<List<AvailabilityDto>>(availabilities));
        }
    }
}
