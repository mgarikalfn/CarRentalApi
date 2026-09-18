using Application.Common;
using Domain.Entities;

namespace Application.Abstractions;

/// <summary>
/// Application-layer repository interface for Payment.
/// Lives in Application (not Domain) because its AddAsync signature
/// declares DuplicateActivePaymentException, which is an Application concern.
/// Infrastructure implements this interface.
/// </summary>
public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Payment>> GetByBookingIdAsync(Guid bookingId, CancellationToken ct = default);

    /// <summary>
    /// Persists a new Payment.
    /// Throws <see cref="DuplicateActivePaymentException"/> if a Pending or Succeeded
    /// payment already exists for the same BookingId (unique constraint violation).
    /// Any other persistence failure propagates as-is.
    /// </summary>
    Task AddAsync(Payment payment, CancellationToken ct = default);

    Task UpdateAsync(Payment payment, CancellationToken ct = default);
}
