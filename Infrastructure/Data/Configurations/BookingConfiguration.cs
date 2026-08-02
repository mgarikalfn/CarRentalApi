using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;


public class BookingConfiguration 
    : IEntityTypeConfiguration<Booking>
{

    public void Configure(EntityTypeBuilder<Booking> builder)
    {

        builder.HasKey(x=>x.Id);



        // =========================
        // Basic Properties
        // =========================


        builder.Property(x=>x.PickUpLocation)
            .HasMaxLength(200)
            .IsRequired();


        builder.Property(x=>x.DropOffLocation)
            .HasMaxLength(200)
            .IsRequired();



        builder.Property(x=>x.Status)
            .HasConversion<string>()
            .IsRequired();



        builder.Property(x=>x.StartDate)
            .IsRequired();


        builder.Property(x=>x.EndDate)
            .IsRequired();



        // =========================
        // Aggregate References
        // =========================


        builder.Property(x=>x.VehicleId)
            .IsRequired();


        builder.Property(x=>x.RenterId)
            .IsRequired();



        builder.HasIndex(x=>x.VehicleId);


        builder.HasIndex(x=>x.RenterId);



        // =========================
        // Booking Price Value Object
        // =========================


        builder.OwnsOne(
            x=>x.Price,
            price =>
            {

                price.Property(x=>x.Subtotal)
                    .HasPrecision(12,2);


                price.Property(x=>x.Discount)
                    .HasPrecision(12,2);


                price.Property(x=>x.InsuranceCost)
                    .HasPrecision(12,2);


                price.Property(x=>x.ServiceFee)
                    .HasPrecision(12,2);


                price.Property(x=>x.TaxRate)
                    .HasPrecision(5,4);


                price.Property(x=>x.Currency)
                    .HasMaxLength(3);

            });



        // =========================
        // Rental Owned Type
        // =========================


        builder.OwnsOne(
            x=>x.Rental,
            rental =>
            {

                rental.Property(x=>x.ActualPickUpTime)
                    .HasColumnName("ActualPickUpTime");


                rental.Property(x=>x.ActualReturnTime)
                    .HasColumnName("ActualReturnTime");


                rental.Property(x=>x.PickUpMileage)
                    .HasColumnName("PickUpMileage");


                rental.Property(x=>x.ReturnMileage)
                    .HasColumnName("ReturnMileage");

            });



        builder.Property(x=>x.CancellationReason)
            .HasMaxLength(500);


        builder.Property(x=>x.RejectionReason)
            .HasMaxLength(500);

    }
}