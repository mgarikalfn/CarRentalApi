using Domain.Abstractions;
using Domain.Common;
using Domain.Entities;
using Domain.Entities.Vehicle;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public sealed class RentalDbContext
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
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
        var aggregates = ChangeTracker
            .Entries<AggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Any())
            .ToList();

        var domainEvents = aggregates
            .SelectMany(e => e.Entity.DomainEvents)
            .ToList();

        var result = await base.SaveChangesAsync(cancellationToken);

        await _dispatcher.DispatchAsync(
            domainEvents,
            cancellationToken);

        foreach (var aggregate in aggregates)
        {
            aggregate.Entity.ClearDomainEvents();
        }

        return result;
    }
}