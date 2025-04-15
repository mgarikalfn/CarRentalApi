namespace CarRentalApi.Entities
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

    public enum PaymentMethod
    {
        CreditCard,
        PayPal,
        BankTransfer
    }

    public enum PaymentStatus
    {
        Pending,
        Completed,
        Refunded,
        Failed
    }

}
