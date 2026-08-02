using Domain.Abstraction;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<RentalDbContext>(options =>
           options.UseNpgsql(
               configuration.GetConnectionString(
                   "DefaultConnection")));

        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<RentalDbContext>()
            .AddDefaultTokenProviders();

        services.AddJwt(configuration);


        services.AddScoped<
            IVehicleRepository,
            VehicleRepository>();

        services.AddScoped<
            IBookingRepository,
            BookingRepository>();

        services.AddScoped<
            IAvailabilityRepository,
            AvailabilityRepository>();

        return services;

    }
}