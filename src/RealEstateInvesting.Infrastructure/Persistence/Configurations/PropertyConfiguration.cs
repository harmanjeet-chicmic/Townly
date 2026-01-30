using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Infrastructure.Persistence.Configurations;

public class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.ToTable("Properties");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.OwnerUserId)
            .IsRequired();

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .IsRequired();

        builder.Property(p => p.Location)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(p => p.PropertyType)
            .IsRequired()
            .HasMaxLength(100);

        // Valuation
        builder.Property(p => p.InitialValuation)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(p => p.ApprovedValuation)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(p => p.TotalUnits)
            .IsRequired();

        builder.Property(p => p.AnnualYieldPercent)
            .HasColumnType("decimal(5,2)")
            .IsRequired();

        // Status
        builder.Property(p => p.Status)
            .IsRequired();

        builder.Property(p => p.ApprovedAt);
        builder.Property(p => p.ImageUrl)
    .HasMaxLength(500);

    }
}
