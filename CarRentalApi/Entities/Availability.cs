namespace CarRentalApi.Entities
{
    public class Availability
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public AvailabilityStatus Status { get; set; } // Available, Blocked, Booked

        public Vehicle Vehicle { get; set; }
    }

    public enum AvailabilityStatus
    {
        Available,
        Blocked,
        Booked
    }

}

