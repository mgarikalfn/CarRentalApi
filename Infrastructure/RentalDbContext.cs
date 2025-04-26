
using System.ComponentModel.DataAnnotations;
using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class RentalDbContext : IdentityDbContext<ApplicationUser>
    {
        public RentalDbContext(DbContextOptions<RentalDbContext> options) : base(options)
        {
        }

        public DbSet<Availability> Availabilities { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<DamageReport> DamageReports { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<VehicleFeatures> VehicleFeatures { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            base.OnModelCreating(modelBuilder);

            // Ignore DisplayFormatAttribute for all entities
            modelBuilder.Ignore<DisplayFormatAttribute>();

            // Configure Identity tables (optional)
            modelBuilder.Entity<ApplicationUser>(b =>
            {
                b.ToTable("Users");
                b.HasMany(u => u.Bookings)
                    .WithOne(b => b.Renter)
                    .HasForeignKey(b => b.RenterId)
                    .OnDelete(DeleteBehavior.Restrict);
            });


            // Vehicle-Feature many-to-many
            modelBuilder.Entity<VehicleFeatures>()
                .HasKey(vf => new { vf.VehicleId, vf.FeatureId });

            modelBuilder.Entity<VehicleFeatures>()
                .HasOne(vf => vf.Vehicle)
                .WithMany(v => v.VehicleFeatures)
                .HasForeignKey(vf => vf.VehicleId);

            modelBuilder.Entity<VehicleFeatures>()
                .HasOne(vf => vf.Feature)
                .WithMany(f => f.VehicleFeatures)
                .HasForeignKey(vf => vf.FeatureId);

            // Booking-Review relationships
            modelBuilder.Entity<Review>()
                .HasOne(r => r.BookingAsVehicleReview)
                .WithOne(b => b.VehicleReview)
                .HasForeignKey<Review>(r => r.VehicleReviewBookingId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.BookingAsRenterReview)
                .WithOne(b => b.RenterReview)
                .HasForeignKey<Review>(r => r.RenterReviewBookingId)
                .OnDelete(DeleteBehavior.Restrict);

            // Vehicle-Booking relationship
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Vehicle)
                .WithMany(v => v.Bookings)
                .HasForeignKey(b => b.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Review relationships
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.GivenReviews)
                .WithOne(r => r.Reviewer)
                .HasForeignKey(r => r.ReviewerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.ReceivedReviews)
                .WithOne(r => r.Reviewee)
                .HasForeignKey(r => r.RevieweeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Vehicle)
                .WithMany()
                .HasForeignKey(r => r.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Vehicle-Images relationship
            modelBuilder.Entity<Vehicle>()
                .HasMany(v => v.Images)
                .WithOne(i => i.Vehicle)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            modelBuilder.Entity<Vehicle>()
                .HasIndex(v => new { v.Make, v.Model });

            modelBuilder.Entity<Availability>()
                .HasIndex(a => new { a.VehicleId, a.StartDate, a.EndDate });
        }
    }
}