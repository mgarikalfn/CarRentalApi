using MediatR;
using Microsoft.AspNetCore.Http;
using Application.Common;

namespace Application.Features.Vehicle.Command
{
    public class CreateVehicleCommand : IRequest<Result<int>>
    {
        public string OwnerId { get; set; } = string.Empty;
        public string Make { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;

        public int Year { get; set; }

        public string LicensePlate { get; set; } = string.Empty;

        public string Color { get; set; } = string.Empty;

        public int Mileage { get; set; }
        public string TransmissionType { get; set; } = string.Empty;

        public string FuelType { get; set; } = string.Empty;

        public int Seats { get; set; }

        public string Description { get; set; } = string.Empty;

       
        public decimal DailyPrice { get; set; }

        public List<IFormFile> Images { get; set; } = new List<IFormFile>();
    }
}
