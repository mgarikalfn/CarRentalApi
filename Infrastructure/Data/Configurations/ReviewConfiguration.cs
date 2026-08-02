using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

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

        builder.Property(r => r.ReviewerId)
            .IsRequired();

        builder.Property(r => r.RevieweeId)
            .IsRequired();

        builder.Property(r => r.Rating)
            .IsRequired();

        builder.Property(r => r.Comment)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        // Helpful indexes
        builder.HasIndex(r => r.VehicleId);

        builder.HasIndex(r => r.BookingId);

        builder.HasIndex(r => r.ReviewerId);

        builder.HasIndex(r => r.RevieweeId);

        builder.HasIndex(r => new { r.VehicleId, r.Status });

        builder.HasIndex(r => new { r.ReviewerId, r.BookingId })
            .IsUnique();
    }
}