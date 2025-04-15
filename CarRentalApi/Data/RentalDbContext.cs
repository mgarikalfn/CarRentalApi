using CarRentalApi.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CarRentalApi.Data
{
    public class RentalDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Availability> Availabilities { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<DamageReport> DamageReports { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<VehicleFeatures> VehicleFeatures { get; set; }

        public RentalDbContext(DbContextOptions<RentalDbContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Ignore<DisplayFormatAttribute>();

            // Vehicle-Feature many-to-many
            modelBuilder.Entity<VehicleFeatures>()
                .HasKey(vf => new { vf.VehicleId, vf.FeatureId });

            // Booking-Review one-to-one relationships
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

            // Corrected User-Booking relationship with proper key types
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Renter)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.RenterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Vehicle-Booking one-to-many
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

            // Indexes
            modelBuilder.Entity<Vehicle>()
                .HasIndex(v => new { v.Make, v.Model });

            modelBuilder.Entity<Availability>()
                .HasIndex(a => new { a.VehicleId, a.StartDate, a.EndDate });
        }
    }
}