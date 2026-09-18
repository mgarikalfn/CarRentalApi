namespace Application.Common;

/// <summary>
/// Thrown by IPaymentRepository.AddAsync when a Pending or Succeeded payment
/// already exists for the given BookingId (unique constraint violation).
/// Application catches this — it never sees PostgresException or DbUpdateException.
/// </summary>
public sealed class DuplicateActivePaymentException : Exception
{
    public DuplicateActivePaymentException()
        : base("A payment is already in progress for this booking.") { }
}
