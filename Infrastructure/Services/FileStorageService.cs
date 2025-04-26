using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging; // Add this using directive to resolve IWebHostEnvironment
namespace Infrastructure.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<FileStorageService> _logger;
        private readonly string _uploadBasePath;

        public FileStorageService(
            IWebHostEnvironment env,
            ILogger<FileStorageService> logger,
            IConfiguration configuration)
        {
            _env = env;
            _logger = logger;

            // Get base path from configuration or use fallback
            _uploadBasePath = configuration["FileStorage:BasePath"] ?? "Uploads";

            // Ensure the upload directory exists
            EnsureUploadDirectoryExists();
        }

        private void EnsureUploadDirectoryExists()
        {
            var fullPath = Path.Combine(_env.ContentRootPath, _uploadBasePath, "vehicles");
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
                _logger.LogInformation($"Created upload directory: {fullPath}");
            }
        }

        public async Task<string> SaveVehicleImageAsync(IFormFile imageFile)
        {
            try
            {
                // Validate input
                if (imageFile == null || imageFile.Length == 0)
                {
                    throw new ArgumentException("No image file provided");
                }

                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var extension = Path.GetExtension(imageFile.FileName)?.ToLowerInvariant();

                if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
                {
                    throw new ArgumentException("Invalid image file type. Only JPG, PNG, or WEBP are allowed.");
                }

                // Validate file size (5MB max)
                if (imageFile.Length > 5 * 1024 * 1024)
                {
                    throw new ArgumentException("Image file size exceeds 5MB limit");
                }

                // Create unique filename
                var fileName = $"{Guid.NewGuid()}{extension}";
                var relativePath = Path.Combine("vehicles", fileName);
                var fullPath = Path.Combine(_env.ContentRootPath, _uploadBasePath, relativePath);

                // Save file
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                _logger.LogInformation($"Saved image to: {fullPath}");

                // Return relative URL path
                return $"/{Path.Combine(_uploadBasePath, relativePath).Replace("\\", "/")}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving vehicle image");
                throw; // Re-throw after logging
            }
        }

        public bool DeleteVehicleImageAsync(string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl))
                {
                    _logger.LogWarning("Attempted to delete image with empty URL");
                    return false;
                }

                // Remove any leading slash and get the relative path
                var relativePath = imageUrl.TrimStart('/');

                // Construct the full path using ContentRootPath (consistent with SaveVehicleImageAsync)
                var fullPath = Path.Combine(_env.ContentRootPath, relativePath);

                if (!File.Exists(fullPath))
                {
                    _logger.LogWarning($"Image file not found at path: {fullPath}");
                    return false;
                }

                File.Delete(fullPath);
                _logger.LogInformation($"Successfully deleted image at path: {fullPath}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting image: {imageUrl}");
                return false;
            }
        }
    }
}
