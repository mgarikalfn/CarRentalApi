using AutoMapper;
using CarRentalApi.Data;
using CarRentalApi.Service;
using MediatR;

namespace CarRentalApi.Application.Vehicle.command
{
    public class DeleteVehicleCommandHandler:IRequestHandler<DeleteVehicleCommand, int>
    {
        private readonly RentalDbContext _context;
        private readonly IFileStorageService _fileStorageService;
        private readonly IMapper _mapper;
        public DeleteVehicleCommandHandler(
            RentalDbContext context,
            IFileStorageService fileStorageService,
            IMapper mapper)
        {
            _context = context;
            _fileStorageService = fileStorageService;
            _mapper = mapper;
        }
       
        public async Task<int> Handle(DeleteVehicleCommand request, CancellationToken cancellationToken)
        {
            var vehicle = await _context.Vehicles.FindAsync(request.Id);
            if (vehicle == null) { return -1; }

            if (vehicle.OwnerId != request.OwnerId)
            {
                return 2;
            }

            // Delete vehicle images from storage
            if (vehicle.Images != null)
            {
                foreach (var image in vehicle.Images)
                {
                    _fileStorageService.DeleteVehicleImageAsync(image.ImageUrl);
                }
            }
            // Remove vehicle from database
            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
            return 1;

        }
    }
   
}
