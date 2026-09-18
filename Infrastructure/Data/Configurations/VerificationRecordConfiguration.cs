using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class VerificationRecordConfiguration : IEntityTypeConfiguration<VerificationRecord>
{
    public void Configure(EntityTypeBuilder<VerificationRecord> builder)
    {
        builder.ToTable("VerificationRecords");

        builder.HasKey(x => x.Id);

        // Explicit FK to ApplicationUser — Guid must match ApplicationUser.Id (uuid).
        // Without this explicit declaration EF would fall back to convention-based
        // inference and, when the property type mismatched (string vs Guid), silently
        // create a shadow UserId1 column with no real FK constraint. Never rely on
        // convention for any FK to ApplicationUser. See ddd-conventions.md.
        builder.Property(x => x.UserId)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(u => u.VerificationRecords)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.Type)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.DocumentNumber)
            .HasMaxLength(100);

        builder.Property(x => x.DocumentUrl)
            .HasMaxLength(500);

        builder.Property(x => x.RejectionReason)
            .HasMaxLength(500);

        builder.Property(x => x.VerifiedAt);

        // VerifiedByUserId is the admin who reviewed the record.
        // No navigation property needed — admin lookup goes through
        // UserManager, not EF navigation. Stored as uuid, nullable.
        builder.Property(x => x.VerifiedByUserId);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.Type);
    }
}
