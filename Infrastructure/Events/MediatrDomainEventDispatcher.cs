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
        foreach(var domainEvent in events)
        {
            await _mediator.Publish(
                domainEvent,
                cancellationToken);
        }
    }
}