using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF configuration for OnChainVaultSupply (Add Property Step 4 audit records).
/// </summary>
public class OnChainVaultSupplyConfiguration : IEntityTypeConfiguration<OnChainVaultSupply>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<OnChainVaultSupply> builder)
    {
        builder.ToTable("OnChainVaultSupplies");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.VaultAddress).IsRequired().HasMaxLength(42);
        builder.Property(x => x.TokenAddress).IsRequired().HasMaxLength(42);
        builder.Property(x => x.AmountMintedRaw).IsRequired().HasMaxLength(78);
        builder.Property(x => x.IdentityTxHash).HasMaxLength(66);
        builder.Property(x => x.MintTxHash).IsRequired().HasMaxLength(66);

        builder.HasIndex(x => x.VaultAddress);
        builder.HasIndex(x => x.TokenAddress);
        builder.HasIndex(x => x.PropertyId);
        builder.HasIndex(x => x.CreatedAt);
    }
}
