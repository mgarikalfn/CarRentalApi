using System.Security.Claims;
using AutoMapper;
using CarRentalApi.Data;
using CarRentalApi.Dto.vehicle;
using CarRentalApi.Entities;
using CarRentalApi.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/vehicles")]
[Authorize]
public class VehiclesController : ControllerBase
{
    private readonly RentalDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IFileStorageService _fileStorageService;
    private readonly IMapper _mapper;

    public VehiclesController(
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

    [HttpPost]
    //[Authorize(Policy = "VehicleOwner")]
    public async Task<ActionResult<Vehicle>> CreateVehicle(
        [FromForm] CreateVehicleDto createVehicleDto)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        // Check if license plate already exists
        if (await _context.Vehicles.AnyAsync(v => v.LicensePlate == createVehicleDto.LicensePlate))
        {
            return BadRequest("A vehicle with this license plate already exists");
        }

        // Map basic properties
        var vehicle = _mapper.Map<Vehicle>(createVehicleDto);
        vehicle.OwnerId = user.Id;

        if (createVehicleDto.Images != null && createVehicleDto.Images.Count > 0)
        {
            vehicle.Images = new List<VehicleImage>();

            for (int i = 0; i < createVehicleDto.Images.Count; i++)
            {
                var imageFile = createVehicleDto.Images[i];
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

        // Return the created vehicle

        var response = _mapper.Map<VehicleDto>(vehicle);
        return Ok(response);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Vehicle>> GetVehicle(int id)
    {
        var vehicle = await _context.Vehicles
            .Include(v => v.Owner)  
            .FirstOrDefaultAsync(v => v.Id == id);

        if (vehicle == null) return NotFound();

        return Ok(_mapper.Map<VehicleDto>(vehicle));
    }
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateVehicle(int id, UpdateVehicleDto vehicleUpdateDto)
    {
        var user = await _userManager.GetUserAsync(User);
        
        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle == null)
        {
            return NotFound();
        }

        if (vehicle.OwnerId != user.Id)
        {
            return Forbid(); 
        }

        _mapper.Map(vehicleUpdateDto, vehicle);

        // Mark as modified and save changes
        _context.Entry(vehicle).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!VehicleExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

    
        var updatedVehicleDto = _mapper.Map<VehicleDto>(vehicle);
        return Ok(updatedVehicleDto);
    }

    private bool VehicleExists(int id)
    {
        return _context.Vehicles.Any(e => e.Id == id);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> DeleteVehicle(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if(user == null) { return  NotFound(); }
        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle == null) { return NotFound(); }

        if (vehicle.OwnerId != user.Id)
        {
            return Forbid();
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
        return NoContent();


    }
}