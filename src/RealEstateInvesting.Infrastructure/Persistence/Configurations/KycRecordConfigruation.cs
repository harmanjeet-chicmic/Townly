using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Infrastructure.Persistence.Configurations;

public class KycRecordConfiguration : IEntityTypeConfiguration<KycRecord>
{
    public void Configure(EntityTypeBuilder<KycRecord> builder)
    {
        builder.ToTable("KycRecords");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.DocumentType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.DocumentUrl)
            .IsRequired();

        builder.Property(x => x.SelfieUrl)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        builder.HasIndex(x => x.UserId);
    }
}
