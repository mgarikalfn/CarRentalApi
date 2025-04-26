using AutoMapper;
using CarRentalApi.Data;
using CarRentalApi.Dto.Availablity;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRentalApi.Application.Availability.Query
{
    public class GetVehicleByIdHandler : IRequestHandler<GetAvailabilityById, ActionResult>
    {
        private readonly RentalDbContext _context;
        private readonly IMapper _mapper;
        public GetVehicleByIdHandler(RentalDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ActionResult> Handle(GetAvailabilityById request, CancellationToken cancellationToken)
        {
            var vehicleExists = await _context.Vehicles.AnyAsync(v => v.Id == request.VehicleId);
            if (!vehicleExists)
            {
                return new NotFoundObjectResult("Vehicle not found");
            }

            var query = _context.Availabilities.Where(a => a.VehicleId == request.VehicleId);
            if (request.StartDate.HasValue)
            {
                query = query.Where(a => a.EndDate >= request.StartDate.Value);
            }
            if (request.EndDate.HasValue)
            {
                query = query.Where(a => a.StartDate <= request.EndDate.Value);
            }

            var availabilities = await query.OrderBy(a => a.StartDate).ToListAsync();
            if (availabilities == null || !availabilities.Any())
            {
                return new NotFoundObjectResult("No availabilities found");
            }

            var availabilityDtos = _mapper.Map<List<AvailabilityDto>>(availabilities);

            return new OkObjectResult( availabilityDtos);
        }

       
    }
}
