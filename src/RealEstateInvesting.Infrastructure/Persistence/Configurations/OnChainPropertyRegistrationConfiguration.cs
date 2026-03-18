using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF configuration for OnChainPropertyRegistration (Flow 4 audit records).
/// </summary>
public class OnChainPropertyRegistrationConfiguration : IEntityTypeConfiguration<OnChainPropertyRegistration>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<OnChainPropertyRegistration> builder)
    {
        builder.ToTable("OnChainPropertyRegistrations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ToAddress).IsRequired().HasMaxLength(42);
        builder.Property(x => x.Uri).IsRequired();
        builder.Property(x => x.TokenAddress).IsRequired().HasMaxLength(42);
        builder.Property(x => x.VaultAddress).IsRequired().HasMaxLength(42);
        builder.Property(x => x.TransactionHash).IsRequired().HasMaxLength(66);

        builder.HasIndex(x => x.PropertyId);
        builder.HasIndex(x => x.TransactionHash);
        builder.HasIndex(x => x.CreatedAt);
    }
}
