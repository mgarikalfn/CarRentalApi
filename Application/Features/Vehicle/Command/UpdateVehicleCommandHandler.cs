

using AutoMapper;
using Domain.Abstraction;
using FluentResults;
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
                return Result.Fail<int>("Request cannot be null").WithError("NULL_REQUEST");

            var vehicle = await _vehicleRepository.GetVehicleByIdAsync(request.Id);
            if (vehicle == null)
                return Result.Fail<int>("Vehicle not found").WithError("NOT_FOUND");

            if (vehicle.OwnerId != request.OwnerId)
                return Result.Fail<int>("Unauthorized vehicle update").WithError("UNAUTHORIZED");

            _mapper.Map(request, vehicle);
            var updateResult = await _vehicleRepository.UpdateVehicleAsync(vehicle);

            if (updateResult.IsSuccess)
                return Result.Ok(vehicle.Id);

            return Result.Fail<int>($"id{updateResult}").WithError("");
        }

    }
}