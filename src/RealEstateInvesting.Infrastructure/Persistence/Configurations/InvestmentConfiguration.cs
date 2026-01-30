using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Infrastructure.Persistence.Configurations;

public class InvestmentConfiguration : IEntityTypeConfiguration<Investment>
{
    public void Configure(EntityTypeBuilder<Investment> builder)
    {
        builder.ToTable("Investments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PricePerShareAtPurchase)
            .HasPrecision(18, 2);

        builder.Property(x => x.TotalAmount)
            .HasPrecision(18, 2);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.PropertyId);
    }
}
