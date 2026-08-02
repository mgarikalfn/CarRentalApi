using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.Data.Configurations;


public class AvailabilityConfiguration 
    : IEntityTypeConfiguration<Availability>
{

    public void Configure(EntityTypeBuilder<Availability> builder)
    {

        builder.HasKey(x=>x.Id);


        builder.Property(x=>x.Type)
            .HasConversion<string>()
            .IsRequired();


        builder.Property(x=>x.Status)
            .HasConversion<string>()
            .IsRequired();



        builder.Property(x=>x.StartDate)
            .IsRequired();



        builder.Property(x=>x.EndDate)
            .IsRequired();



        builder.HasIndex(x=>x.VehicleId);



        // Important for searching availability
        builder.HasIndex(
            x=>new 
            {
                x.VehicleId,
                x.StartDate,
                x.EndDate
            });

    }
}