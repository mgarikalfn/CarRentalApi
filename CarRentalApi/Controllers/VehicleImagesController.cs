using Application.Dto.vehicle;
using Domain.Abstraction;
using Domain.Entities;
using Domain.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

[Route("api/vehicles/{vehicleId}/images")]
[ApiController]
public class VehicleImagesController : ControllerBase
{
    private readonly IFileStorageService _fileStorage;
    private readonly IVehicleRepository _vehicleRepository;

    public VehicleImagesController(IFileStorageService fileStorage, IVehicleRepository vehicleRepository)
    {
        _fileStorage = fileStorage;
        _vehicleRepository = vehicleRepository;
    }

    [HttpPost]
    public async Task<ActionResult<VehicleImageDto>> UploadVehicleImage(
        [FromRoute] Guid vehicleId,
        [FromForm] UploadVehicleImageDto uploadDto)
    {
        if (uploadDto?.ImageFile == null || uploadDto.ImageFile.Length == 0)
            return BadRequest("No valid image file provided.");

        var vehicle = await _vehicleRepository.GetVehicleByIdAsync(vehicleId);
        if (vehicle == null)
            return NotFound("Vehicle not found");

        var imageUrl = await _fileStorage.SaveVehicleImageAsync(uploadDto.ImageFile);
        vehicle.AddPhoto(imageUrl, uploadDto.DisplayOrder);
        await _vehicleRepository.UpdateVehicleAsync(vehicle);

        return Ok(new VehicleImageDto { ImageUrl = imageUrl });
    }

    [HttpGet]
    public async Task<ActionResult<List<VehicleImageDto>>> GetVehicleImages([FromRoute] Guid vehicleId)
    {
        var vehicle = await _vehicleRepository.GetVehicleByIdAsync(vehicleId);
        if (vehicle == null)
            return NotFound("Vehicle not found");

        var imageDtos = vehicle.Photos.Select(p => new VehicleImageDto { ImageUrl = p.Url }).ToList();
        return Ok(imageDtos);
    }

    [HttpDelete("{photoId}")]
    public async Task<IActionResult> DeleteVehicleImage(
        [FromRoute] Guid vehicleId,
        [FromRoute] Guid photoId)
    {
        var vehicle = await _vehicleRepository.GetVehicleByIdAsync(vehicleId);
        if (vehicle == null)
            return NotFound("Vehicle not found");

        vehicle.RemovePhoto(photoId);
        await _vehicleRepository.UpdateVehicleAsync(vehicle);
        return NoContent();
    }
}
