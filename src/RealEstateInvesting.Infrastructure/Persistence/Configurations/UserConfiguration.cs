using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.WalletAddress)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.WalletAddress)
            .IsUnique();

        builder.Property(x => x.ChainId)
            .IsRequired();

        builder.Property(x => x.Role)
            .IsRequired();

        builder.Property(x => x.KycStatus)
            .IsRequired();

        builder.Property(x => x.IsBlocked)
            .IsRequired();
    }
}
