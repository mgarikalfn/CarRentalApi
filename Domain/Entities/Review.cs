using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public string ReviewerId { get; set; }
        public ApplicationUser Reviewer { get; set; }

        public string RevieweeId { get; set; }
        public ApplicationUser Reviewee { get; set; }

        // Other properties
        public int? VehicleReviewBookingId { get; set; }
        public Booking BookingAsVehicleReview { get; set; }

        public int? RenterReviewBookingId { get; set; }
        public Booking BookingAsRenterReview { get; set; }

        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }
    }
}
