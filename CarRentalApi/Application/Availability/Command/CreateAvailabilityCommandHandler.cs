using CarRentalApi.Data;
using CarRentalApi.Dto.Availablity;
using CarRentalApi.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace CarRentalApi.Application.Availability.Command
{
    public class CreateAvailabilityCommandHandler : IRequestHandler<CreateAvailabilityCommand, CreateAvailabilityResponseDto>
    {
        private readonly RentalDbContext _context;
        private readonly IMapper _mapper;

        public CreateAvailabilityCommandHandler(RentalDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CreateAvailabilityResponseDto> Handle(CreateAvailabilityCommand request, CancellationToken cancellationToken)
        {
            var response = new CreateAvailabilityResponseDto();

            // Validate vehicle exists
            var vehicle = await _context.Vehicles.FindAsync(request.VehicleId);
            if (vehicle == null)
            {
                response.ErrorMessage = "Vehicle not found";
                response.StatusCode = 404;
                return response;
            }

            // Validate ownership
            if (vehicle.OwnerId != request.OwnerId)
            {
                response.ErrorMessage = "You don't have permission to modify this vehicle";
                response.StatusCode = 403;
                return response;
            }

            // Validate date range
            if (request.EndDate <= request.StartDate)
            {
                response.ErrorMessage = "End date must be after start date";
                response.StatusCode = 400;
                return response;
            }

            // Check for overlapping availabilities
            var overlapping = await _context.Availabilities
                .Where(a => a.VehicleId == request.VehicleId)
                .AnyAsync(a => a.StartDate < request.EndDate && a.EndDate > request.StartDate,
                          cancellationToken);

            if (overlapping)
            {
                response.ErrorMessage = "The specified date range overlaps with existing availability";
                response.StatusCode = 409;
                return response;
            }

            // Map and create availability
            var availability = _mapper.Map<Entities.Availability>(request);
            availability.Status = AvailabilityStatus.Blocked;

            _context.Availabilities.Add(availability);
            await _context.SaveChangesAsync(cancellationToken);

            // Return successful response
            response.AvailabilityId = availability.Id;
            response.StatusCode = 201;
            return response;
        }
    }

    public class CreateAvailabilityResponseDto
    {
        public int AvailabilityId { get; set; }
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}