using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Infrastructure.Persistence.Configurations;

public class PropertyDocumentConfiguration : IEntityTypeConfiguration<PropertyDocument>
{
    public void Configure(EntityTypeBuilder<PropertyDocument> builder)
    {
        builder.ToTable("PropertyDocuments");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.PropertyId)
            .IsRequired();

        builder.Property(d => d.DocumentName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.DocumentUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(d => d.UploadedAt)
            .IsRequired();
    }
}
