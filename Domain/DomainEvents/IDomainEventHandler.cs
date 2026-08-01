using Domain.Common;

namespace Domain.Entities;

public interface IDomainEventHandler<in TEvent> where  TEvent:IDomainEvent
{
    Task Handle(TEvent domainEvent);
}