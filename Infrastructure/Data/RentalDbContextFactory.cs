using Infrastructure.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data;

/// <summary>
/// Design-time factory used by `dotnet ef migrations` when the application
/// startup project cannot be used directly (e.g. missing DI registrations).
/// This factory is NOT used at runtime.
/// </summary>
public class RentalDbContextFactory : IDesignTimeDbContextFactory<RentalDbContext>
{
    public RentalDbContext CreateDbContext(string[] args)
    {
        // Build configuration from the CarRentalApi project's appsettings
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "CarRentalApi"))
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Database=CarRentalDb;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<RentalDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        // Use a no-op dispatcher for design-time; events don't fire during migrations
        return new RentalDbContext(optionsBuilder.Options, new NoOpDomainEventDispatcher());
    }
}
