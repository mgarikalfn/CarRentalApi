using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Reviews");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.BookingId)
            .IsRequired();

        builder.Property(r => r.VehicleId)
            .IsRequired();

        builder.Property(r => r.Rating)
            .IsRequired();

        builder.Property(r => r.Comment)
            .HasMaxLength(1000);

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .IsRequired();

        // IsFlaggedForReview: set by any authenticated user via Flag();
        // does NOT affect Status, which is admin-only.
        builder.Property(r => r.IsFlaggedForReview)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(r => r.FlaggedAt);

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        // ── FK #1: ReviewerId → ApplicationUser (the review writer) ──────
        // Explicitly declared on Review side to prevent shadow FK column.
        // ddd-conventions.md §EF Core FK rule: two FKs to same table MUST
        // both be explicit or EF produces ReviewerId1/RevieweeId1 shadows.
        builder.Property(r => r.ReviewerId)
            .IsRequired();
        builder.HasOne<ApplicationUser>()
            .WithMany(u => u.GivenReviews)
            .HasForeignKey(r => r.ReviewerId)
            .OnDelete(DeleteBehavior.Restrict);

        // ── FK #2: RevieweeId → ApplicationUser (the review subject) ─────
        builder.Property(r => r.RevieweeId)
            .IsRequired();
        builder.HasOne<ApplicationUser>()
            .WithMany(u => u.ReceivedReviews)
            .HasForeignKey(r => r.RevieweeId)
            .OnDelete(DeleteBehavior.Restrict);

        // ── FK #3: VehicleId → Vehicle ────────────────────────────────────
        // No navigation property on Review — Guid reference only.
        // Explicit configuration prevents convention-based inference.
        builder.HasOne<Vehicle>()
            .WithMany()
            .HasForeignKey(r => r.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

        // ── Idempotency: one review per reviewer per booking ──────────────
        builder.HasIndex(r => new { r.BookingId, r.ReviewerId })
            .IsUnique()
            .HasDatabaseName("UX_Reviews_BookingId_ReviewerId");

        // ── Additional indexes ────────────────────────────────────────────
        builder.HasIndex(r => r.RevieweeId);
        builder.HasIndex(r => r.VehicleId);
        builder.HasIndex(r => r.Status);
    }
}
