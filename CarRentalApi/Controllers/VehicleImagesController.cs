using AutoMapper;
using CarRentalApi.Data;
using CarRentalApi.Dto.vehicle;
using CarRentalApi.Entities;
using CarRentalApi.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/vehicles/{vehicleId}/images")]
[ApiController]
public class VehicleImagesController : ControllerBase
{
    private readonly IFileStorageService _fileStorage;
    private readonly RentalDbContext _context;
    private readonly IMapper _mapper;

    public VehicleImagesController(
        IFileStorageService fileStorage,
        RentalDbContext context,
        IMapper mapper)
    {
        _fileStorage = fileStorage;
        _context = context;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult<VehicleImageDto>> UploadVehicleImage(
        [FromRoute] int vehicleId,
        [FromForm] UploadVehicleImageDto uploadDto)
    {
        if (uploadDto?.ImageFile == null || uploadDto.ImageFile.Length == 0)
        {
            return BadRequest("No valid image file provided.");
        }

        var vehicle = await _context.Vehicles
            .Include(v => v.Images)
            .FirstOrDefaultAsync(v => v.Id == vehicleId);

        if (vehicle == null)
        {
            return NotFound("Vehicle not found");
        }

        try
        {
            var imageUrl = await _fileStorage.SaveVehicleImageAsync(uploadDto.ImageFile);

            var newImage = new VehicleImage
            {
                VehicleId = vehicleId,
                ImageUrl = imageUrl,
                IsPrimary = uploadDto.IsPrimary,
                DisplayOrder = uploadDto.DisplayOrder
            };

            if (newImage.IsPrimary || vehicle.Images.Count == 0)
            {
                foreach (var existingImage in vehicle.Images)
                {
                    existingImage.IsPrimary = false;
                }
                newImage.IsPrimary = true;
            }

            vehicle.Images.Add(newImage);
            await _context.SaveChangesAsync();

            // Use AutoMapper to map the entity to DTO
            var imageDto = _mapper.Map<VehicleImageDto>(newImage);
            return Ok(imageDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error uploading image: {ex.Message}");
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<VehicleImageDto>>> GetVehicleImages(int vehicleId)
    {
        var vehicle = await _context.Vehicles
            .Include(v => v.Images)
            .FirstOrDefaultAsync(v => v.Id == vehicleId);

        if (vehicle == null)
        {
            return NotFound("Vehicle not found");
        }

        // Use AutoMapper to map the list of entities to DTOs
        var imageDtos = _mapper.Map<List<VehicleImageDto>>(vehicle.Images);
        return Ok(imageDtos);
    }

    [HttpDelete("{imageId}")]
    public async Task<IActionResult> DeleteVehicleImage(
        [FromRoute] int vehicleId,
        [FromRoute] int imageId)
    {
        var vehicle = await _context.Vehicles
            .Include(v => v.Images)
            .FirstOrDefaultAsync(v => v.Id == vehicleId);
        if (vehicle == null)
        {
            return NotFound("Vehicle not found");
        }
        var image = vehicle.Images.FirstOrDefault(i => i.Id == imageId);
        if (image == null)
        {
            return NotFound("Image not found");
        }
        // Delete the image from storage
        var isDeleted = _fileStorage.DeleteVehicleImageAsync(image.ImageUrl);
        if (!isDeleted)
        {
            return StatusCode(500, "Error deleting image from storage");
        }
        vehicle.Images.Remove(image);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}