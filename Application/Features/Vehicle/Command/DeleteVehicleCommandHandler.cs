using AutoMapper;
using Application.Common;
using Domain.Abstraction;
using Domain.Services.Interfaces;
using MediatR;

namespace Application.Features.Vehicle.Command
{
    public class DeleteVehicleCommandHandler : IRequestHandler<DeleteVehicleCommand, Result<int>>
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IMapper _mapper;
        
        public DeleteVehicleCommandHandler(
            IVehicleRepository vehicleRepository,
            IFileStorageService fileStorageService,
            IMapper mapper)
        {
            _vehicleRepository = vehicleRepository;
            _fileStorageService = fileStorageService;
            _mapper = mapper;
        }

        public async Task<Result<int>> Handle(DeleteVehicleCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
                return Result<int>.Failure("Request cannot be null", "NULL_REQUEST");

            var deleteSuccess = await _vehicleRepository.DeleteVehicleAsync(request.Id, request.OwnerId);

            return deleteSuccess
                ? Result<int>.Success(1)
                : Result<int>.Failure($"Failed to delete vehicle", "DELETE_FAILED");
        }
    }
}
