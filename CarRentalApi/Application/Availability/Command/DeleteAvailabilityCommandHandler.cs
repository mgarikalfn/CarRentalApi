using CarRentalApi.Data;
using CarRentalApi.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarRentalApi.Application.Availability.Command
{
    public class DeleteAvailabilityCommandHandler : IRequestHandler<DeleteAvailabilityCommand, int>
    {
        private readonly RentalDbContext _context;
        public DeleteAvailabilityCommandHandler(RentalDbContext context)
        {
            _context = context;
        }
        public async Task<int> Handle(DeleteAvailabilityCommand request, CancellationToken cancellationToken)
        {
            // Check if vehicle exists
            var vehicle = await _context.Vehicles.FindAsync(request.VehicleId);
            if (vehicle == null)
            {
                return -1;
            }

            var availability = await _context.Availabilities
                .FirstOrDefaultAsync(a => a.Id == request.Id && a.VehicleId == request.VehicleId);

            if (availability == null)
            {
                return -2;
            }

            if (availability.Status != AvailabilityStatus.Blocked)
            {
                return 2;
            }

            _context.Availabilities.Remove(availability);
            return availability.Id;
        }
    }
}
