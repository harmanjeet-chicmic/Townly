using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Infrastructure.Persistence.Configurations;

/// <summary>EF configuration for OnChainShareSale (Flow 6 audit).</summary>
public class OnChainShareSaleConfiguration : IEntityTypeConfiguration<OnChainShareSale>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<OnChainShareSale> builder)
    {
        builder.ToTable("OnChainShareSales");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PropertyTokenAddress).IsRequired().HasMaxLength(42);
        builder.Property(x => x.AmountOfSharesRaw).IsRequired().HasMaxLength(78);
        builder.Property(x => x.SellTxHash).IsRequired().HasMaxLength(66);
        builder.Property(x => x.ApproveTxHash).HasMaxLength(66);
        builder.Property(x => x.UserWalletAddress).HasMaxLength(42);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.PropertyTokenAddress);
        builder.HasIndex(x => x.CreatedAt);
    }
}
