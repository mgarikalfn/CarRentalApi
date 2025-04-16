namespace CarRentalApi.Service
{
    public interface IFileStorageService
    {
        Task<string> SaveVehicleImageAsync(IFormFile imageFile);
        bool  DeleteVehicleImageAsync(string imageUrl);
    }
}
