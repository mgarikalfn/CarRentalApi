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
    // Rental is an owned type embedded inside Booking — no separate DbSet.
    public DbSet<Availability> Availabilities => Set<Availability>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<DamageReport> DamageReports => Set<DamageReport>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(
            typeof(RentalDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        if (_isDispatching)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        try
        {
            _isDispatching = true;

            var domainEventEntities = ChangeTracker
                .Entries<IHasDomainEvents>()
                .Where(entry => entry.Entity.DomainEvents.Any())
                .ToList();

            var domainEvents = domainEventEntities
                .SelectMany(entry => entry.Entity.DomainEvents)
                .ToList();

            var result = await base.SaveChangesAsync(cancellationToken);

            foreach (var entity in domainEventEntities)
            {
                entity.Entity.ClearDomainEvents();
            }

            await _dispatcher.DispatchAsync(domainEvents, cancellationToken);

            return result;
        }
        finally
        {
            _isDispatching = false;
        }
    }
}
