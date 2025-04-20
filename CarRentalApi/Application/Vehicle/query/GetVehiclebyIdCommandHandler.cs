using AutoMapper;
using CarRentalApi.Data;
using CarRentalApi.Dto.vehicle;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarRentalApi.Application.Vehicle.query
{
    public class GetVehiclebyIdCommandHandler:IRequestHandler<GetVehicleByIdCommand, VehicleDto>
    {
        private readonly RentalDbContext _context;
        private readonly IMapper _mapper;
        public GetVehiclebyIdCommandHandler(RentalDbContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<VehicleDto> Handle(GetVehicleByIdCommand request, CancellationToken cancellationToken)
        {
            var vehicle = await _context.Vehicles
           .Include(v => v.Owner)
           .FirstOrDefaultAsync(v => v.Id == request.Id);

            var result = _mapper.Map<VehicleDto>(vehicle);

            return result;
        }
    }
   
}
