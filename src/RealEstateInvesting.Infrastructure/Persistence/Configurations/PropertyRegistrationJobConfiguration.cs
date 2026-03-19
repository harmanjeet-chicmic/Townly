using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Infrastructure.Persistence.Configurations;

public class PropertyRegistrationJobConfiguration : IEntityTypeConfiguration<PropertyRegistrationJob>
{
    public void Configure(EntityTypeBuilder<PropertyRegistrationJob> builder)
    {
        builder.ToTable("PropertyRegistrationJobs");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Status).HasColumnName("status");
        builder.Property(x => x.PropertyId).HasColumnName("property_id");
        builder.Property(x => x.OwnerAddress).HasColumnName("owner_address");
        builder.Property(x => x.PricePerShare).HasColumnName("price_per_share");
        builder.Property(x => x.MintAmount).HasColumnName("mint_amount");
        builder.Property(x => x.IpfsUri).HasColumnName("ipfs_uri");
        builder.Property(x => x.PropertyName).HasColumnName("property_name");
        builder.Property(x => x.Description).HasColumnName("description");
        builder.Property(x => x.Location).HasColumnName("location");
        builder.Property(x => x.PropertyType).HasColumnName("property_type");
        builder.Property(x => x.TrexDeployTxHash).HasColumnName("trex_deploy_tx_hash");
        builder.Property(x => x.TokenAddress).HasColumnName("token_address");
        builder.Property(x => x.VaultAddress).HasColumnName("vault_address");
        builder.Property(x => x.OnChainPropertyId).HasColumnName("on_chain_property_id");
        builder.Property(x => x.ErrorMessage).HasColumnName("error_message");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
    }
}
