namespace CarRentalApi.Dto.vehicle
{
    
    public class VehicleFilterDto
    {
        // Basic filters
        public string? Make { get; set; }
        public string? Model { get; set; }
        public int? MinYear { get; set; }
        public int? MaxYear { get; set; }

        // Price range
        public decimal? MinDailyPrice { get; set; }
        public decimal? MaxDailyPrice { get; set; }

        // Vehicle type
        public string? TransmissionType { get; set; }
        public string? FuelType { get; set; }
        public int? MinSeats { get; set; }

        // Availability
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // Features
        public List<int> FeatureIds { get; set; } = new List<int>();

        // Sorting
        public string SortBy { get; set; } = "Make"; // Default sort
        public bool SortDescending { get; set; } = false;

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
