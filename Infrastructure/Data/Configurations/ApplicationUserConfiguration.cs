using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class ApplicationUserConfiguration 
    : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("Users");


        // Identity configuration
        builder.Property(x => x.FirstName)
            .HasMaxLength(50)
            .IsRequired();


        builder.Property(x => x.LastName)
            .HasMaxLength(50)
            .IsRequired();


        builder.Property(x => x.ProfilePhotoUrl)
            .HasMaxLength(500);


        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();


        builder.Property(x => x.CreatedAt)
            .IsRequired();


        // Trust Profile one-to-one
        builder.HasOne(x => x.TrustProfile)
            .WithOne(x => x.User)
            .HasForeignKey<TrustProfile>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);


        // User -> Vehicles
        builder.HasMany(x => x.OwnedVehicles)
            .WithOne()
            .HasForeignKey(x => x.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);


        // User -> Bookings
        builder.HasMany(x => x.Bookings)
            .WithOne()
            .HasForeignKey(x => x.RenterId)
            .OnDelete(DeleteBehavior.Restrict);


        // User -> Payments
        builder.HasMany(x => x.Payments)
            .WithOne()
            .HasForeignKey(x => x.PayerId)
            .OnDelete(DeleteBehavior.Restrict);


        // Note: GivenReviews and ReceivedReviews FK relationships are declared
        // on the Review side (ReviewConfiguration) using explicit .HasOne<ApplicationUser>()
        // .WithMany(u => u.GivenReviews/.ReceivedReviews) .HasForeignKey(...).
        // Do NOT redeclare them here — duplicate declarations produce extra shadow FK columns.

        builder.HasIndex(x => x.Status);
    }
}