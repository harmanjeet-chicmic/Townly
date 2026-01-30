using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Infrastructure.Persistence.Configurations;

public class WalletNonceConfiguration : IEntityTypeConfiguration<WalletNonce>
{
    public void Configure(EntityTypeBuilder<WalletNonce> builder)
    {
        builder.ToTable("WalletNonces");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.WalletAddress)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Nonce)
            .IsRequired();

        builder.Property(x => x.ExpiresAt)
            .IsRequired();

        builder.HasIndex(x => x.WalletAddress);
        builder.HasIndex(x => x.ExpiresAt);
    }
}
