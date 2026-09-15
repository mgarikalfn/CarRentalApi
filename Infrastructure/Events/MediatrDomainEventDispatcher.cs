using Application.Common;
using Domain.Abstractions;
using Domain.Common;
using MediatR;

namespace Infrastructure.Events;

public sealed class MediatrDomainEventDispatcher
    : IDomainEventDispatcher
{
    private readonly IMediator _mediator;

    public MediatrDomainEventDispatcher(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task DispatchAsync(
        IEnumerable<IDomainEvent> events,
        CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in events)
        {
            // Wrap each domain event in DomainEventNotification<T>
            // so MediatR can dispatch it to INotificationHandler<DomainEventNotification<T>>
            var wrapperType = typeof(DomainEventNotification<>)
                .MakeGenericType(domainEvent.GetType());
            var notification = (INotification)Activator.CreateInstance(wrapperType, domainEvent)!;
            
            await _mediator.Publish(notification, cancellationToken);
        }
    }
}