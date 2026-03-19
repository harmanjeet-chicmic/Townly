using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateInvesting.Domain.Entities;

namespace RealEstateInvesting.Infrastructure.Persistence.Configurations;

public class PropertyActivationRecordConfiguration : IEntityTypeConfiguration<PropertyActivationRecord>
{
    public void Configure(EntityTypeBuilder<PropertyActivationRecord> builder)
    {
        builder.ToTable("PropertyActivationRecords");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.JobId).IsRequired();
        builder.Property(x => x.PropertyId).IsRequired();
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.TrexDeployTxHash).HasMaxLength(66);
        builder.Property(x => x.CreatedBy).IsRequired();

        builder.HasIndex(x => x.JobId);
        builder.HasIndex(x => x.PropertyId);
        builder.HasIndex(x => x.CreatedAt);
    }
}
