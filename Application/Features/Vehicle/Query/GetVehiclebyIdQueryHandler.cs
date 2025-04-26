
using Application.Dto.vehicle;
using AutoMapper;
using Domain.Abstraction;
using FluentResults;
using MediatR;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Application.Features.Vehicle
{
    public class GetVehiclebyIdQueryHandler:IRequestHandler<GetVehicleByIdQuery, Result<VehicleDto>>
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IMapper _mapper;
        public GetVehiclebyIdQueryHandler(IVehicleRepository vehicleRepository,IMapper mapper)
        {
            _vehicleRepository = vehicleRepository;
            _mapper = mapper;
        }
        

        public async Task<Result<VehicleDto>> Handle(GetVehicleByIdQuery request, CancellationToken cancellationToken)
        {
            var vehicle = await _vehicleRepository.GetVehicleByIdAsync(request.Id);

            return vehicle == null
                ? Result.Fail<VehicleDto>("Vehicle not found")
                : Result.Ok(_mapper.Map<VehicleDto>(vehicle));
        }
    }
   
}
