using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Infrastructure.Persistence.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.AmountUsd)
               .HasPrecision(18, 2)
               .IsRequired();

        builder.Property(t => t.EthAmountAtExecution)
               .HasPrecision(18, 8);

        builder.Property(t => t.EthUsdRateAtExecution)
               .HasPrecision(18, 6);

        builder.Property(t => t.Currency)
               .HasMaxLength(10)
               .IsRequired();

        builder.Property(t => t.IsSuccessful)
               .IsRequired();
    }
}
