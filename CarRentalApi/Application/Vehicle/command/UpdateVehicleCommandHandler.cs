using AutoMapper;
using CarRentalApi.Data;
using CarRentalApi.Dto.vehicle;
using CarRentalApi.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CarRentalApi.Application.Vehicle.command
{
    public class UpdateVehicleCommandHandler:IRequestHandler<UpdateVehicleCommand, int>
    {
        private readonly RentalDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        public UpdateVehicleCommandHandler(RentalDbContext context, IMapper mapper,UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }
        public async Task<int> Handle(UpdateVehicleCommand request, CancellationToken cancellationToken)
        {
            
            var vehicle = await _context.Vehicles.FindAsync(request.Id);
            if (vehicle == null)
            {
                return -1;
            }

            if (vehicle.OwnerId != request.OwnerId)
            {
                return -2;
            }

            _mapper.Map(request, vehicle);

            // Mark as modified and save changes
            _context.Entry(vehicle).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VehicleExists(request.Id))
                {
                    return -1;
                }
                else
                {
                    throw;
                }
            }
            return 1;
        }

        private bool VehicleExists(int id)
        {
            return _context.Vehicles.Any(e => e.Id == id);
        }
    }
   
}
