using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Infrastructure.Persistence.Configurations;

public class TokenPurchaseConfiguration : IEntityTypeConfiguration<TokenPurchase>
{
    public void Configure(EntityTypeBuilder<TokenPurchase> builder)
    {
        builder.ToTable("TokenPurchases");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Status).HasColumnName("status");
        builder.Property(x => x.PropertyId).HasColumnName("property_id");
        builder.Property(x => x.TransactionHash).HasColumnName("transaction_hash");
        builder.Property(x => x.BuyerAddress).HasColumnName("buyer_address");
        builder.Property(x => x.SellerAddress).HasColumnName("seller_address");
        builder.Property(x => x.Shares).HasColumnName("shares");
        builder.Property(x => x.Amount).HasColumnName("amount");
        builder.Property(x => x.ErrorMessage).HasColumnName("error_message");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at");
    }
}
