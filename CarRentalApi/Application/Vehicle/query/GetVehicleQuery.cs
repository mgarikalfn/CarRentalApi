using CarRentalApi.Dto.vehicle;
using CarRentalApi.Entities;
using MediatR;

namespace CarRentalApi.Application.Vehicle.query
{
    public class GetVehicleQuery : IRequest<List<VehicleDto>>
    {
        public int VehicleId { get; set; }
        public string OwnerId { get; set; }
        public string RenterId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPrice { get; set; }
        public BookingStatus Status { get; set; }
    }
}
