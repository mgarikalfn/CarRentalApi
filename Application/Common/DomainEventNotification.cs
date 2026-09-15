using MediatR;
using Domain.Common;

namespace Application.Common;

/// <summary>
/// Adapter that wraps a domain event as an INotification for MediatR dispatch.
/// This lives in the Application layer — Domain has ZERO knowledge of MediatR.
/// 
/// The dispatch flow is:
/// 1. Domain aggregate raises event via AddDomainEvent(...)
/// 2. SaveChangesAsync() persists the aggregate to the database
/// 3. AppDbContext.SaveChangesAsync override wraps each domain event in
///    DomainEventNotification<T> and publishes via MediatR, AFTER
///    base.SaveChangesAsync() has succeeded
/// 4. INotificationHandler<DomainEventNotification<T>> implementations run
/// 5. AppDbContext.SaveChangesAsync clears events via ClearDomainEvents()
/// </summary>
/// <typeparam name="TDomainEvent">The domain event type being wrapped.</typeparam>
public class DomainEventNotification<TDomainEvent> : INotification
    where TDomainEvent : IDomainEvent
{
    /// <summary>
    /// The wrapped domain event.
    /// </summary>
    public TDomainEvent DomainEvent { get; }

    /// <summary>
    /// Wraps a domain event for MediatR dispatch.
    /// </summary>
    /// <param name="domainEvent">The domain event to wrap. Cannot be null.</param>
    /// <exception cref="ArgumentNullException">Thrown if domainEvent is null.</exception>
    public DomainEventNotification(TDomainEvent domainEvent)
    {
        DomainEvent = domainEvent ?? throw new ArgumentNullException(nameof(domainEvent));
    }
}
