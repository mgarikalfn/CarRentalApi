using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public int PayerId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public PaymentMethod Method { get; set; }
        public string TransactionId { get; set; }
        public PaymentStatus Status { get; set; }

        public Booking Booking { get; set; }
        public ApplicationUser Payer { get; set; }
    }
}
