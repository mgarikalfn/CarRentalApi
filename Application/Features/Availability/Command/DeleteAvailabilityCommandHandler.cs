
using Domain.Abstraction;
using Domain.Enums;
using FluentResults;
using MediatR;

namespace Application.Features.Availability.Command
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
                return Result.Fail<int>("Request cannot be null");

            var vehicle = await _vehicleRepository.GetVehicleByIdAsync(request.VehicleId);
            if (vehicle == null)
                return Result.Fail<int>("Vehicle doesn't exist");

            var availability = await _availabilityRepository.GetAvailabilityByVehicleIdAsync(
                request.Id, request.VehicleId, cancellationToken);

            if (availability == null)
                return Result.Fail<int>("Availability can't be found");

            if (availability.Status != AvailabilityStatus.Blocked)
                return Result.Fail<int>("Can't remove availability at this stage");

            var deleteResult = await _availabilityRepository.DeleteAvailabilityAsync(
                request.Id, string.Empty, cancellationToken);

            return deleteResult.IsSuccess
                ? Result.Ok(request.Id)
                : Result.Fail<int>("Failed to delete availability");
        }
    }
}
