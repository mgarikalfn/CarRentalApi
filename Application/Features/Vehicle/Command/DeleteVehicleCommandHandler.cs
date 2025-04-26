using AutoMapper;
using Domain.Abstraction;
using Domain.Services.Interfaces;
using MediatR;
using FluentResults;

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
            // 1. Validate input
            if (request == null)
                return Result.Fail<int>("Request cannot be null").WithError("NULL_REQUEST");
            //return Result<int>.Fai("Request cannot be null", "NULL_REQUEST");

            // 2. Call repository
            var deleteResult = await _vehicleRepository.DeleteVehicleAsync(request.Id, request.OwnerId);

            // 3. Transform repository result to handler response
            return deleteResult.IsSuccess
                ? Result.Ok(request.Id)
                : Result.Fail<int>($"id{request.Id}").WithError("");
        }
    }
   
}
