using Domain.Entities.Vehicle;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.Data.Configurations;


public class VehicleConfiguration 
    : IEntityTypeConfiguration<Vehicle>
{

    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {

        builder.HasKey(x => x.Id);


        // Owner reference
        // No ApplicationUser navigation because DDD boundary

        builder.Property(x => x.OwnerId)
            .IsRequired();



        builder.Property(x => x.Title)
            .HasMaxLength(100)
            .IsRequired();


        builder.Property(x => x.Description)
            .HasMaxLength(1000);



        builder.Property(x => x.Status)
            .HasConversion<string>()
            .IsRequired();



        builder.Property(x => x.AverageRating)
            .HasPrecision(3,2);



        // ============================
        // Vehicle Specification
        // ============================

        builder.OwnsOne(
            x => x.Specification,
            specification =>
            {

                specification.Property(x=>x.Brand)
                    .HasColumnName("Brand")
                    .HasMaxLength(50)
                    .IsRequired();


                specification.Property(x=>x.Model)
                    .HasColumnName("Model")
                    .HasMaxLength(50)
                    .IsRequired();


                specification.Property(x=>x.Year)
                    .HasColumnName("Year")
                    .IsRequired();


                specification.Property(x=>x.Color)
                    .HasColumnName("Color")
                    .HasMaxLength(50);


                specification.Property(x=>x.FuelType)
                    .HasConversion<string>();


                specification.Property(x=>x.Transmission)
                    .HasConversion<string>();


                specification.Property(x=>x.VIN)
                    .HasColumnName("VIN")
                    .HasMaxLength(17)
                    .IsRequired();


                specification.Property(x=>x.LicensePlate)
                    .HasColumnName("LicensePlate")
                    .HasMaxLength(15)
                    .IsRequired();


                specification.Property(x=>x.SeatCount)
                    .IsRequired();

            });



        // ============================
        // Money Value Object
        // ============================


        builder.OwnsOne(
            x=>x.Price,
            money =>
            {

                money.Property(x=>x.DailyPrice)
                    .HasColumnName("DailyPrice")
                    .HasPrecision(10,2);


                money.Property(x=>x.Currency)
                    .HasColumnName("Currency")
                    .HasMaxLength(3)
                    .IsRequired();

            });



        // ============================
        // GeoLocation
        // ============================


        builder.OwnsOne(
            x=>x.Location,
            location =>
            {

                location.Property(x=>x.Latitude)
                    .HasColumnName("Latitude");


                location.Property(x=>x.Longitude)
                    .HasColumnName("Longitude");


                location.Property(x=>x.City)
                    .HasColumnName("City")
                    .HasMaxLength(100);

            });



        // ============================
        // Photos
        // ============================

        builder.OwnsMany(
            x=>x.Photos,
            photo =>
            {

                photo.ToTable("VehiclePhotos");


                photo.HasKey(x=>x.Id);


                photo.Property(x=>x.Url)
                    .HasMaxLength(500)
                    .IsRequired();


                photo.Property(x=>x.IsPrimary)
                    .IsRequired();



                photo.WithOwner()
                    .HasForeignKey("VehicleId");

            });



        builder.HasIndex(x=>x.OwnerId);


        builder.HasIndex(x=>x.Status);


        builder.HasIndex(x=>x.AverageRating);
        
        builder.OwnsMany(
            x => x.Photos,
            photo =>
            {
                photo.ToTable("VehiclePhotos");


                photo.HasKey(x => x.Id);


                photo.Property(x => x.Url)
                    .HasMaxLength(500)
                    .IsRequired();


                photo.Property(x => x.Type)
                    .HasConversion<string>()
                    .IsRequired();


                photo.Property(x => x.IsPrimary)
                    .IsRequired();


                photo.Property(x => x.UploadedAt)
                    .IsRequired();


                photo.WithOwner()
                    .HasForeignKey(x => x.VehicleId);
            });
    }
}