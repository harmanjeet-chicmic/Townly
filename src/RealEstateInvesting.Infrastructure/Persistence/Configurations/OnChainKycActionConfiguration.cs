using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Infrastructure.Persistence.Configurations;

public class OnChainKycActionConfiguration : IEntityTypeConfiguration<OnChainKycAction>
{
    public void Configure(EntityTypeBuilder<OnChainKycAction> builder)
    {
        builder.ToTable("OnChainKycActions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.WalletAddress)
            .IsRequired()
            .HasMaxLength(42);

        builder.Property(x => x.TransactionHash)
            .IsRequired()
            .HasMaxLength(66);

        builder.Property(x => x.IdentityContractAddress)
            .HasMaxLength(42);

        builder.HasIndex(x => x.WalletAddress);
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.CreatedAt);
    }
}
