using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.Entities
{
    public class DamageReport:BaseEntity
    {


     
        public int BookingId { get; set; }      // Associated booking
        public int VehicleId { get; set; }      // Damaged vehicle
        public int ReporterId { get; set; }     // User who reported (owner/renter/admin)

        // Damage details
        public string Description { get; set; }
        public string ImageUrl { get; set; }    // Photo of damage
        public DateTime ReportedAt { get; set; } = DateTime.UtcNow;
        public DamageSeverity Severity { get; set; } // Minor, Moderate, Severe
        public decimal RepairCost { get; set; } // Estimated cost
        public bool IsResolved { get; set; }    // Marked true after resolution

        // Navigation properties
        public Booking Booking { get; set; }
        public Vehicle Vehicle { get; set; }
        public ApplicationUser Reporter { get; set; }
    }
}
