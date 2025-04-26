
using Domain.Abstraction;
using FluentResults;
using MediatR;

namespace Application.Features.Availability.Command
{  
    public class DeleteAvailabilityCommandHandler : IRequestHandler<DeleteAvailabilityCommand, Result<int>>
    {
        public readonly IAvailabilityRepository _availabilityRepository;
        private readonly IVehicleRepository _vehicleRepository;
        public DeleteAvailabilityCommandHandler(IAvailabilityRepository availabilityRepository,IVehicleRepository vehicleRepository)
        {
            _availabilityRepository = availabilityRepository;
            _vehicleRepository = vehicleRepository;
        }
        public async Task<int> Handle(DeleteAvailabilityCommand request, CancellationToken cancellationToken)
        {
            // Check if vehicle exists
            var vehicle = await _context.Vehicles.FindAsync(request.VehicleId);
            if (vehicle == null)
            {
                return -1;
            }

            var availability = await _context.Availabilities
                .FirstOrDefaultAsync(a => a.Id == request.Id && a.VehicleId == request.VehicleId);

            if (availability == null)
            {
                return -2;
            }

            if (availability.Status != AvailabilityStatus.Blocked)
            {
                return 2;
            }

            _context.Availabilities.Remove(availability);
            return availability.Id;
        }

        async Task<Result<int>> IRequestHandler<DeleteAvailabilityCommand, Result<int>>.Handle(DeleteAvailabilityCommand request, CancellationToken cancellationToken)
        {

            if (request == null)
                return Result.Fail<int>("Request cannot be null");

            var vehicle = await _vehicleRepository.GetVehicleByIdAsync(request.VehicleId);
            if (vehicle == null)
                return Result.Fail<int>("Vehicle doesn't exist");

            var availability = await _availabilityRepository.GetAvailabilityByVehicleIdAsync(request.Id, request.VehicleId, cancellationToken);

            if (availability == null)
                return Result.Fail<int>("availability can't be found");

            if (availability.Status != Domain.Enums.AvailabilityStatus.Blocked)
                return Result.Fail<int>("can't be removed at this stage");


        }
    }
}
