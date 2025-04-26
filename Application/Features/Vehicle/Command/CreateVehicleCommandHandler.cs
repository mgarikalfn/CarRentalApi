using AutoMapper;
using Domain.Abstraction;
using Domain.Common;
using Domain.Entities;
using Domain.Services.Interfaces;
using MediatR;

namespace Application.Features.Vehicle.Command
{
    public class CreateVehicleCommandHandler : IRequestHandler<CreateVehicleCommand, Result<int>>
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly IMapper _mapper;

        private readonly IVehicleRepository _vehicleRepository;
        public CreateVehicleCommandHandler(
            IFileStorageService fileStorageService,
            IMapper mapper,
            IVehicleRepository vehicleRepository)
        {
            _fileStorageService = fileStorageService;
            _mapper = mapper;

            _vehicleRepository = vehicleRepository;
        }

        public async Task<Result<int>> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
        {
            // 1. Validate input
            if (request == null)
                return Result<int>.Failure("Request cannot be null", "VALIDATION_ERROR");

            // 2. Check business rules
            if (await _vehicleRepository.VehicleExistsAsync(request.LicensePlate))
                return Result<int>.Failure("Vehicle with this license plate already exists", "DUPLICATE_VEHICLE");

            // 3. Map and process
            var vehicle = _mapper.Map<Domain.Entities.Vehicle>(request);

            if (request.Images?.Count > 0)
            {
                vehicle.Images = new List<VehicleImage>();

                for (int i = 0; i < request.Images.Count; i++)
                {
                    var imageUrl = await _fileStorageService.SaveVehicleImageAsync(request.Images[i]);
                    vehicle.Images.Add(new VehicleImage
                    {
                        ImageUrl = imageUrl,
                        IsPrimary = i == 0,
                        DisplayOrder = i
                    });
                }
            }

            // 4. Persist
            var vehicleId = await _vehicleRepository.CreateVehicleAsync(vehicle);

            return Result<int>.Success(vehicleId);
        }
    }
    
}
