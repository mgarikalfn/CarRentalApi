using AutoMapper;
using CarRentalApi.Data;
using CarRentalApi.Dto.vehicle;
using CarRentalApi.Entities;
using CarRentalApi.Service;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CarRentalApi.Application.Vehicle.command
{
    public class CreateVehicleCommandHandler : IRequestHandler<CreateVehicleCommand, int>
    {
        private readonly RentalDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileStorageService _fileStorageService;
        private readonly IMapper _mapper;
        public CreateVehicleCommandHandler(
            RentalDbContext context,
            UserManager<ApplicationUser> userManager,
            IFileStorageService fileStorageService,
            IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _fileStorageService = fileStorageService;
            _mapper = mapper;
        }
       
       public  async Task<int>Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
        {
            
            // Check if license plate already exists
            if (await _context.Vehicles.AnyAsync(v => v.LicensePlate == request.LicensePlate))
            {
                return -1;
            }

            // Map basic properties
            var vehicle =_mapper.Map<Entities.Vehicle>(request);
            

            if (request != null && request.Images.Count > 0)
            {
                vehicle.Images = new List<VehicleImage>();

                for (int i = 0; i < request.Images.Count; i++)
                {
                    var imageFile = request.Images[i];
                    var imageUrl = await _fileStorageService.SaveVehicleImageAsync(imageFile);

                    vehicle.Images.Add(new VehicleImage
                    {
                        ImageUrl = imageUrl,
                        IsPrimary = i == 0, // First image is primary
                        DisplayOrder = i
                    });
                }
            }

            // Add to database
            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();
            return vehicle.Id;
        }
   
    }
}
