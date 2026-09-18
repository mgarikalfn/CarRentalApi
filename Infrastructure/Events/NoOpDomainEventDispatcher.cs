using Domain.Abstractions;
using Domain.Common;

namespace Infrastructure.Events;

/// <summary>
/// No-operation dispatcher used at design time (migrations).
/// Events are silently discarded — this is only wired in
/// RentalDbContextFactory, never at runtime.
/// </summary>
internal sealed class NoOpDomainEventDispatcher : IDomainEventDispatcher
{
    public Task DispatchAsync(
        IEnumerable<IDomainEvent> domainEvents,
        CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}
