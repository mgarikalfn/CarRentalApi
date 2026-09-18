using AutoMapper;
using Application.Common;
using Domain.Abstraction;
using Domain.Common;
using Domain.Entities;
using Domain.Services.Interfaces;
using MediatR;

namespace Application.Features.Vehicles.Command
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

            // 3. Create vehicle with domain factory
            var specification = new VehicleSpecification(
                request.Make,
                request.Model,
                request.Year,
                request.Color,
                System.Enum.Parse<Domain.Entities.FuelType>(request.FuelType),
                System.Enum.Parse<Domain.Entities.TransmissionType>(request.TransmissionType),
                "UNKNOWN_VIN",
                request.LicensePlate,
                request.Seats);

            var price = new Money(request.DailyPrice); // DailyPrice is a field on the command DTO, passed as `amount` to Money ctor

            var vehicle = Domain.Entities.Vehicle.Create(
                System.Guid.Parse(request.OwnerId),
                request.Make + " " + request.Model,
                request.Description ?? "",
                specification,
                price,
                request.Mileage);

            // 4. Add photos
            if (request.Images?.Count > 0)
            {
                for (int i = 0; i < request.Images.Count; i++)
                {
                    var imageUrl = await _fileStorageService.SaveVehicleImageAsync(request.Images[i]);
                    vehicle.AddPhoto(imageUrl, i);
                }
            }

            // 5. Persist
            var vehicleId = await _vehicleRepository.CreateVehicleAsync(vehicle);

            return Result<int>.Success(vehicleId);
        }
    }
    
}
