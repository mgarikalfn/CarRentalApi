
using AutoMapper;
using Domain.Abstraction;
using Domain.Enums;
using FluentResults;
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

      
         async Task<Result<int>> IRequestHandler<CreateAvailabilityCommand, Result<int>>.Handle(CreateAvailabilityCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
                return Result.Fail<int>("Request cannot be null");

           var vehicle = await _vehicleRepository.GetVehicleByIdAsync(request.VehicleId);
            if (vehicle == null)
                return Result.Fail<int>("Vehicle doesn't exist");
            if (vehicle.OwnerId != request.OwnerId)
                return Result.Fail<int>("Unauthorized access");

            if (request.EndDate <= request.StartDate)
                return Result.Fail<int>("End date must be after start date");

            var isOverlapping = await _availabilityRepository.HasOverlappingAvailabilityAsync(request.VehicleId, request.StartDate, request.EndDate,cancellationToken);


            if(isOverlapping)
            {
                return Result.Fail<int>("Overlapping availability exists");
            }

            var availability = _mapper.Map<Domain.Entities.Availability>(request);
            availability.Status = AvailabilityStatus.Blocked;

            var result = await _availabilityRepository.CreateAvailabilityRepository(availability);
            return Result.Ok(result);


        }
    }
}