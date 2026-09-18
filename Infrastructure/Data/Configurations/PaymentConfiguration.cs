using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.BookingId).IsRequired();
        builder.Property(x => x.PayerId).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();

        // Money value object — owned columns on Payments table.
        // Property named PaidAmount on Payment to avoid payment.Amount.Amount collision.
        // Column names are explicit to avoid EF's default PaidAmount_Amount prefix.
        builder.OwnsOne(x => x.PaidAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("Amount")
                .HasPrecision(12, 2)
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Property(x => x.Method)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.TransactionReference).HasMaxLength(200);
        builder.Property(x => x.FailureReason).HasMaxLength(500);
        builder.Property(x => x.RefundReason).HasMaxLength(500);

        builder.Property(x => x.SucceededAt);
        builder.Property(x => x.FailedAt);
        builder.Property(x => x.RefundedAt);

        // Idempotency: at most one Pending or Succeeded payment per booking.
        // Failed and Refunded rows are excluded so a retry is allowed after failure/refund.
        builder.HasIndex(x => x.BookingId)
            .IsUnique()
            .HasFilter("\"Status\" NOT IN ('Failed', 'Refunded')")
            .HasDatabaseName("UX_Payments_BookingId_Active");

        builder.HasIndex(x => x.PayerId);
        builder.HasIndex(x => x.Status);
    }
}
