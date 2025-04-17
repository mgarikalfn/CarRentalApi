namespace CarRentalApi.Dto.vehicle
{
    public class UploadVehicleImageDto
    {
        public IFormFile ImageFile { get; set; }
        public bool IsPrimary { get; set; } = false;
        public int DisplayOrder { get; set; } = 0;
    }
}
