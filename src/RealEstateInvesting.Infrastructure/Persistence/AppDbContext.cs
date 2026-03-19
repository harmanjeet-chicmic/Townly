using Microsoft.EntityFrameworkCore;
using RealEstateInvesting.Domain.Common;
using RealEstateInvesting.Domain.Entities;
using System.Reflection;

namespace RealEstateInvesting.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<KycRecord> KycRecords => Set<KycRecord>();
    public DbSet<Property> Properties => Set<Property>();
    public DbSet<Investment> Investments => Set<Investment>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<WalletNonce> WalletNonces => Set<WalletNonce>();
    public DbSet<PropertyDocument> PropertyDocuments => Set<PropertyDocument>();
    public DbSet<PropertyUpdateRequest> PropertyUpdateRequests => Set<PropertyUpdateRequest>();
    public DbSet<PropertyAnalyticsSnapshot> PropertyAnalyticsSnapshots
    => Set<PropertyAnalyticsSnapshot>();

    public DbSet<UserPortfolioSnapshot> UserPortfolioSnapshots
        => Set<UserPortfolioSnapshot>();
    public DbSet<PropertyImage> PropertyImages => Set<PropertyImage>();
    public DbSet<AdminUser> Admins { get; set; } = default!;
    public DbSet<Log> Logs { get; set; }


    // ADD THESE
    public DbSet<TokenRequest> TokenRequests => Set<TokenRequest>();
    public DbSet<UserTokenBalance> UserTokenBalances => Set<UserTokenBalance>();
    public DbSet<TokenTransaction> TokenTransactions => Set<TokenTransaction>();
    public DbSet<UserDeviceToken> UserDeviceTokens { get; set; } = default!;
    public DbSet<OnChainKycAction> OnChainKycActions => Set<OnChainKycAction>();
    public DbSet<OnChainPropertyRegistration> OnChainPropertyRegistrations => Set<OnChainPropertyRegistration>();
    public DbSet<OnChainSharePurchase> OnChainSharePurchases => Set<OnChainSharePurchase>();
    public DbSet<OnChainShareSale> OnChainShareSales => Set<OnChainShareSale>();
    public DbSet<OnChainVaultSupply> OnChainVaultSupplies => Set<OnChainVaultSupply>();
    public DbSet<PropertyActivationRecord> PropertyActivationRecords => Set<PropertyActivationRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ApplySoftDeleteQueryFilter(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            Assembly.GetExecutingAssembly());
    }

    // -----------------------------
    // Soft delete global filter
    // -----------------------------
    private static void ApplySoftDeleteQueryFilter(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(AppDbContext)
                    .GetMethod(nameof(SetSoftDeleteFilter),
                        BindingFlags.NonPublic | BindingFlags.Static)!
                    .MakeGenericMethod(entityType.ClrType);

                method.Invoke(null, new object[] { modelBuilder });
            }
        }
    }

    private static void SetSoftDeleteFilter<TEntity>(ModelBuilder builder)
        where TEntity : BaseEntity
    {
        builder.Entity<TEntity>()
            .HasQueryFilter(e => !e.IsDeleted);
    }
}
