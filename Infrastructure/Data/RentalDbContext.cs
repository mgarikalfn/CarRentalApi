using Domain.Abstractions;
using Domain.Common;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public sealed class RentalDbContext
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    private bool _isDispatching = false;
    private readonly IDomainEventDispatcher _dispatcher;

    public RentalDbContext(
        DbContextOptions<RentalDbContext> options,
        IDomainEventDispatcher dispatcher)
        : base(options)
    {
        _dispatcher = dispatcher;
    }

    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Rental> Rentals => Set<Rental>();
    public DbSet<Availability> Availabilities => Set<Availability>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(
            typeof(RentalDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        // Reentrancy Guard: If already dispatching, skip dispatch logic
        // Nested SaveChangesAsync calls (from event handlers) just persist and return
        if (_isDispatching)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        try
        {
            _isDispatching = true;

            // 1. Snapshot all domain events from IHasDomainEvents implementers
            // (catches both AggregateRoot and ApplicationUser)
            var domainEventEntities = ChangeTracker
                .Entries<IHasDomainEvents>()
                .Where(entry => entry.Entity.DomainEvents.Any())
                .ToList();

            var domainEvents = domainEventEntities
                .SelectMany(entry => entry.Entity.DomainEvents)
                .ToList();

            // 2. Persist changes to database
            var result = await base.SaveChangesAsync(cancellationToken);

            // 3. Clear events AFTER persisting but BEFORE dispatching
            // NOTE: For operations without an explicit ambient transaction (e.g., Vehicle.UpdatePrice,
            // Booking.Approve), EF Core's implicit auto-transaction means the DB write above is ALREADY
            // permanently committed at this point. If DispatchAsync below throws, the event is lost even
            // though the data change stands—a known limitation for MVP.
            // This is safe only for flows explicitly wrapped in BeginTransactionAsync (e.g., user
            // registration), where a dispatch failure rolls back the entire transaction.
            // PLANNED FIX: Outbox pattern (persist events atomically with data, dispatch via worker).
            foreach (var entity in domainEventEntities)
            {
                entity.Entity.ClearDomainEvents();
            }

            // 4. Dispatch events via MediatrDomainEventDispatcher
            // Handlers run synchronously. If a handler calls SaveChangesAsync(),
            // _isDispatching=true prevents re-dispatch (reentrancy guard)
            await _dispatcher.DispatchAsync(domainEvents, cancellationToken);

            return result;
        }
        finally
        {
            _isDispatching = false;
        }
    }
}