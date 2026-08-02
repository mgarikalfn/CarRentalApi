using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class TrustProfileConfiguration : IEntityTypeConfiguration<TrustProfile>
{
    public void Configure(EntityTypeBuilder<TrustProfile> builder)
    {
        builder.ToTable("TrustProfiles");

        builder.HasKey(tp => tp.Id);

        // -------------------------
        // Properties
        // -------------------------

        builder.Property(tp => tp.UserId)
            .IsRequired();

        builder.Property(tp => tp.OverallScore)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(tp => tp.VerificationLevel)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(tp => tp.CompletedRentals)
            .HasDefaultValue(0);

        builder.Property(tp => tp.CancelledBookings)
            .HasDefaultValue(0);

        builder.Property(tp => tp.LateReturns)
            .HasDefaultValue(0);

        builder.Property(tp => tp.DamageClaimsTotal)
            .HasDefaultValue(0);

        builder.Property(tp => tp.DamageClaimsAtFault)
            .HasDefaultValue(0);

        builder.Property(tp => tp.AverageRatingAsRenter)
            .HasPrecision(3, 2);

        builder.Property(tp => tp.AverageRatingAsHost)
            .HasPrecision(3, 2);

        builder.Property(tp => tp.ResponseRate)
            .HasPrecision(5, 2);

        builder.Property(tp => tp.AvgResponseTimeMinutes);

        builder.Property(tp => tp.LastRecalculatedAt);

        builder.Property(tp => tp.IsUnderReview)
            .HasDefaultValue(false);

        builder.Property(tp => tp.ReviewReason)
            .HasMaxLength(500);

        builder.Property(tp => tp.FlaggedAt);

        // -------------------------
        // One-to-One User
        // -------------------------

        builder.HasOne(tp => tp.User)
            .WithOne()
            .HasForeignKey<TrustProfile>(tp => tp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // -------------------------
        // Indexes
        // -------------------------

        builder.HasIndex(tp => tp.UserId)
            .IsUnique();

        builder.HasIndex(tp => tp.OverallScore);

        builder.HasIndex(tp => tp.VerificationLevel);

        builder.HasIndex(tp => tp.IsUnderReview);
    }
}