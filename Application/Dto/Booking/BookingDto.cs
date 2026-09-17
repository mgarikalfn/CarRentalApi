using Application.Dto.User;
using Application.Dto.vehicle;
using Domain.Enums;

namespace Application.Dto.Booking
{
    public class BookingDto
    {
        public Guid Id { get; set; }
        public Guid VehicleId { get; set; }
        public Guid RenterId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public BookingStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

        // Flattened from BookingPrice value object
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal InsuranceCost { get; set; }
        public decimal ServiceFee { get; set; }
        public decimal TaxRate { get; set; }
        public decimal Total { get; set; }
        public string Currency { get; set; } = "BIRR";

        public string? CancellationReason { get; set; }
        public string? RejectionReason { get; set; }

        // Hydrated by handler if needed — not navigation properties on aggregate
        public VehicleDto? Vehicle { get; set; }
        public UserDto? Renter { get; set; }
    }
}
