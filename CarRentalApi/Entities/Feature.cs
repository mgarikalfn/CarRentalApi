namespace CarRentalApi.Entities
{
    public class Feature
    {
            public int Id { get; set; }
            public string Name { get; set; } // "GPS", "Sunroof", "Child Seat"
            public string Icon { get; set; }

            public ICollection<VehicleFeatures> VehicleFeatures { get; set; }
        


    }
}
