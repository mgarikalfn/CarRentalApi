using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class PaymentConfiguration
    : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.BookingId)
            .IsRequired();

        builder.Property(x => x.PayerId)
            .IsRequired();

        builder.Property(x => x.Amount)
            .HasPrecision(12, 2)
            .IsRequired();

        builder.Property(x => x.Currency)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(x => x.Method)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.TransactionReference)
            .HasMaxLength(200);

        builder.Property(x => x.CompletedAt);

        builder.Property(x => x.RefundedAt);

        builder.HasIndex(x => x.BookingId);

        builder.HasIndex(x => x.PayerId);

        builder.HasIndex(x => x.Status);

        builder.HasIndex(x => x.TransactionReference)
            .IsUnique(false);
    }
}