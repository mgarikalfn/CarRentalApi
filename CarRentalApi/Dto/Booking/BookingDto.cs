using CarRentalApi.Dto.User;
using CarRentalApi.Dto.vehicle;
using CarRentalApi.Entities;

namespace CarRentalApi.Dto.Booking
{
    public class BookingDto
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public string RenterId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPrice { get; set; }
        public BookingStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public VehicleDto Vehicle { get; set; }
        public UserDto Renter { get; set; }
    }
}
