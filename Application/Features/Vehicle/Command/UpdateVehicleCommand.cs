using Application.Common;
using MediatR;

namespace Application.Features.Vehicle.Command
{
    public class UpdateVehicleCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public Guid OwnerId { get; set; }

        public string? Make { get; set; }
        public string? Model { get; set; }
        public int? Year { get; set; }
        public string? LicensePlate { get; set; }
        public string? Color { get; set; }
        public int? Mileage { get; set; }
        public string? TransmissionType { get; set; }
        public string? FuelType { get; set; }
        public int? Seats { get; set; }
        public string? Description { get; set; }
        public decimal? DailyPrice { get; set; }
    }
}
