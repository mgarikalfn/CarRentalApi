namespace CarRentalApi.Dto.vehicle
{
    public class VehicleImageDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public bool IsPrimary { get; set; }
        public int DisplayOrder { get; set; }
        public int VehicleId { get; set; }
    }
}
