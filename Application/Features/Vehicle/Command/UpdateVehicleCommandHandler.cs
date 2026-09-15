using AutoMapper;
using Application.Common;
using Domain.Abstraction;
using MediatR;

namespace Application.Features.Vehicle.Command
{
    public class UpdateVehicleCommandHandler : IRequestHandler<UpdateVehicleCommand, Result<int>>
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IMapper _mapper;
       
        public UpdateVehicleCommandHandler(IMapper mapper, IVehicleRepository vehicleRepository)
        {
            _mapper = mapper;
            _vehicleRepository = vehicleRepository;
        }
        
        public async Task<Result<int>> Handle(UpdateVehicleCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
                return Result<int>.Failure("Request cannot be null", "NULL_REQUEST");

            var vehicle = await _vehicleRepository.GetVehicleByIdAsync(request.Id);
            if (vehicle == null)
                return Result<int>.Failure("Vehicle not found", "NOT_FOUND");

            if (vehicle.OwnerId != request.OwnerId)
                return Result<int>.Failure("Unauthorized vehicle update", "UNAUTHORIZED");

            _mapper.Map(request, vehicle);
            var updateSuccess = await _vehicleRepository.UpdateVehicleAsync(vehicle);

            if (updateSuccess)
                return Result<int>.Success((int)vehicle.Id);

            return Result<int>.Failure("Failed to update vehicle", "UPDATE_FAILED");
        }
    }
}