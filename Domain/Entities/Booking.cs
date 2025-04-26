using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.Entities
{
    public class Booking:BaseEntity
    {
        
        public int VehicleId { get; set; }
        public string RenterId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPrice { get; set; }
        public BookingStatus Status { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public Vehicle Vehicle { get; set; }
        public ApplicationUser Renter { get; set; }
        public ICollection<Payment> Payments { get; set; }
        public Review VehicleReview { get; set; }
        public Review RenterReview { get; set; }
        public DamageReport DamageReport { get; set; }
    }
}
