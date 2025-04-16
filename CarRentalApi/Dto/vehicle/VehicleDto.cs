namespace CarRentalApi.Dto.vehicle
{
    public class VehicleDto
    {
        public int Id { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string LicensePlate { get; set; }
        public string Color { get; set; }
        // Include other properties you need

        // Simplified owner info
        public OwnerDto Owner { get; set; }
    }
}
