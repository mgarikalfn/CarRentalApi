using Application.Common;
using Domain.Abstraction;
using Domain.Enums;
using MediatR;

namespace Application.Features.Availabilities.Command
{
    public class DeleteAvailabilityCommandHandler : IRequestHandler<DeleteAvailabilityCommand, Result<int>>
    {
        private readonly IAvailabilityRepository _availabilityRepository;
        private readonly IVehicleRepository _vehicleRepository;

        public DeleteAvailabilityCommandHandler(
            IAvailabilityRepository availabilityRepository,
            IVehicleRepository vehicleRepository)
        {
            _availabilityRepository = availabilityRepository;
            _vehicleRepository = vehicleRepository;
        }

        public async Task<Result<int>> Handle(DeleteAvailabilityCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
                return Result<int>.Failure("Request cannot be null");

            var vehicle = await _vehicleRepository.GetVehicleByIdAsync(request.VehicleId);
            if (vehicle == null)
                return Result<int>.Failure("Vehicle doesn't exist");

            var availability = await _availabilityRepository.GetAvailabilityByVehicleIdAsync(
                request.Id, request.VehicleId, cancellationToken);

            if (availability == null)
                return Result<int>.Failure("Availability can't be found");

            if (availability.Status != AvailabilityStatus.Active)
                return Result<int>.Failure("Can't remove availability at this stage");

            var deleteResult = await _availabilityRepository.DeleteAvailabilityAsync(
                request.Id, string.Empty, cancellationToken);

            if (!deleteResult.Value)
                return Result<int>.Failure("Failed to delete availability");

            return Result<int>.Success(request.Id);
        }
    }
}
