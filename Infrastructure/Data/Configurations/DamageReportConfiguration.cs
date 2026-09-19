using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class DamageReportConfiguration : IEntityTypeConfiguration<DamageReport>
{
    public void Configure(EntityTypeBuilder<DamageReport> builder)
    {
        builder.ToTable("DamageReports");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.BookingId).IsRequired();
        builder.Property(d => d.VehicleId).IsRequired();
        builder.Property(d => d.CreatedAt).IsRequired();

        builder.Property(d => d.Description)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(d => d.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(d => d.ResolvedAt);

        // ── FK: ReportedByUserId → ApplicationUser ─────────────────────────
        // Explicit — same mandatory pattern as ReviewerId/RevieweeId on Review.
        // Without this, EF convention would silently create a shadow property
        // if the type ever drifted. See ddd-conventions.md.
        builder.Property(d => d.ReportedByUserId).IsRequired();
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(d => d.ReportedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // ── FK: VehicleId → Vehicle ────────────────────────────────────────
        builder.HasOne<Vehicle>()
            .WithMany()
            .HasForeignKey(d => d.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

        // ── FK: BookingId → Booking ────────────────────────────────────────
        builder.HasOne<Booking>()
            .WithMany()
            .HasForeignKey(d => d.BookingId)
            .OnDelete(DeleteBehavior.Restrict);

        // ── DamageImage owned collection ───────────────────────────────────
        // OwnsMany = no separate Id, no independent table — images are embedded.
        builder.OwnsMany(d => d.Images, image =>
        {
            image.ToTable("DamageReportImages");
            image.WithOwner().HasForeignKey("DamageReportId");
            image.Property<int>("Id")
                 .ValueGeneratedOnAdd();
            image.HasKey("Id");
            image.Property(i => i.Url)
                 .HasMaxLength(500)
                 .IsRequired();
            image.Property(i => i.UploadedAt).IsRequired();
        });

        // ── Indexes ────────────────────────────────────────────────────────
        builder.HasIndex(d => d.BookingId);
        builder.HasIndex(d => d.VehicleId);
        builder.HasIndex(d => d.ReportedByUserId);
        builder.HasIndex(d => d.Status);
    }
}
