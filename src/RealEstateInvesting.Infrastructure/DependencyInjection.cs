using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealEstateInvesting.Application.Auth.Interfaces;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Infrastructure.Identity;
using RealEstateInvesting.Infrastructure.Persistence;
using RealEstateInvesting.Infrastructure.Persistence.Repositories;
using RealEstateInvesting.Application.Kyc;
using RealEstateInvesting.Infrastructure.Kyc;
using RealEstateInvesting.Application.Admin.Kyc;
using RealEstateInvesting.Application.Admin.Kyc.Interfaces;
using RealEstateInvesting.Application.Kyc.Queries;
using RealEstateInvesting.Infrastructure.Kyc.ReadModels;
using RealEstateInvesting.Infrastructure.Admin.Kyc;
using RealEstateInvesting.Infrastructure.Admin.Users;
using RealEstateInvesting.Infrastructure.Health;
using RealEstateInvesting.Infrastructure.Storage;
using RealEstateInvesting.Infrastructure.Push;
using RealEstateInvesting.Infrastructure.Pricing;
using RealEstateInvesting.Infrastructure.Admin.Properties;
using RealEstateInvesting.Infrastructure.Security;
using RealEstateInvesting.Application.AdminAuth.Interfaces;
using RealEstateInvesting.Application.AdminAuth;
using RealEstateInvesting.Application.Admin.Properties.Interfaces;
using RealEstateInvesting.Application.Admin.Properties;
using RealEstateInvesting.Application.Admin.Users;
using RealEstateInvesting.Application.Admin.Users.Interfaces;
using RealEstateInvesting.Application.Notifications.Interfaces;
using RealEstateInvesting.Application.Notifications;
using RealEstateInvesting.Application.Health.Queries;
using RealEstateInvesting.Infrastructure.Blockchain;
using RealEstateInvesting.Application.Tokens.Requests;
using RealEstateInvesting.Application.Tokens.Balance;
using RealEstateInvesting.Application.Properties.PropertyRegistrationApi;
using RealEstateInvesting.Infrastructure.PropertyRegistrationApi;
using Amazon.S3;
using Microsoft.Extensions.Caching.Memory;
namespace RealEstateInvesting.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));

        // Auth & Identity
        services.AddScoped<IWalletNonceService, WalletNonceService>();
        services.AddScoped<IWalletAuthService, WalletAuthService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<IAdminAuthService, AdminAuthService>();
        services.AddScoped<IAdminRepository, AdminRepository>();
        services.AddScoped<IAdminPasswordHasher, AdminPasswordHasher>();

        // Core Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPropertyRepository, PropertyRepository>();
        services.AddScoped<IInvestmentRepository, InvestmentRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // KYC System
        services.AddScoped<IKycRecordRepository, KycRecordRepository>();
        services.AddScoped<IAdminKycRepository, AdminKycRepository>();
        services.AddScoped<IAdminKycService, AdminKycService>();
        services.AddScoped<IKycFileStorageService, KycFileStorageService>();
        services.AddScoped<IGetMyKycStatusReadService, KycStatusReadService>();
        services.AddScoped<SubmitKycHandler>();

        // Notifications
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IPushNotificationService, PushNotificationService>();
        services.AddScoped<IUserDeviceTokenRepository, UserDeviceTokenRepository>();

        // Properties & Analytics
        services.AddScoped<IAdminPropertyRepository, AdminPropertyRepository>();
        services.AddScoped<IAdminPropertyService, AdminPropertyService>();
        services.AddScoped<IPropertyDocumentRepository, PropertyDocumentRepository>();
        services.AddScoped<IPropertyImageRepository, PropertyImageRepository>();
        services.AddScoped<IPropertyUpdateRequestRepository, PropertyUpdateRequestRepository>();
        services.AddScoped<IAnalyticsSnapshotRepository, AnalyticsSnapshotRepository>();
        services.AddScoped<IAdminUserRepository, AdminUserRepository>();
        services.AddScoped<IAdminUserService, AdminUserService>();
        
        // External & Infrastructure
        services.AddScoped<IHealthCheckService, HealthCheckService>();
        services.AddScoped<ILogRepository, LogRepository>();
        services.AddHttpClient<IPriceFeed, CoinGeckoPriceFeed>();
        services.AddMemoryCache();
        services.AddHttpClient<CoinGeckoEthPriceService>();

        // External T-REX property registration API (POST /v1/property-register)
        services.Configure<PropertyRegistrationApiOptions>(configuration.GetSection(PropertyRegistrationApiOptions.SectionName));
        services.AddHttpClient<IPropertyRegistrationApiClient, PropertyRegistrationApiClient>();
        
        services.AddScoped<IEthPriceService>(sp =>
        {
            var live = sp.GetRequiredService<CoinGeckoEthPriceService>();
            var cache = sp.GetRequiredService<IMemoryCache>();
            return new CachedEthPriceService(live, cache);
        });

        // S3 Storage
        var awsSection = configuration.GetSection("AWS");
        services.AddSingleton<IAmazonS3>(_ =>
        {
            return new AmazonS3Client(
                awsSection["AccessKey"],
                awsSection["SecretKey"],
                Amazon.RegionEndpoint.GetBySystemName(awsSection["Region"])
            );
        });

        services.AddScoped<IFileStorage>(sp =>
        {
            return new S3FileStorage(
                sp.GetRequiredService<IAmazonS3>(),
                awsSection["BucketName"]!,
                awsSection["BasePrefix"]!,
                awsSection["Region"]!
            );
        });

        // Token System
        services.AddScoped<ITokenRequestRepository, TokenRequestRepository>();
        services.AddScoped<IUserTokenBalanceRepository, UserTokenBalanceRepository>();
        services.AddScoped<ITokenTransactionRepository, TokenTransactionRepository>();
        services.AddScoped<CreateTokenRequestHandler>();
        services.AddScoped<ReviewTokenRequestHandler>();
        services.AddScoped<UserTokenBalanceService>();

        // T-REX Identity Registry (on-chain KYC) + Real Estate Registry (Flow 4)
        services.Configure<TRexOptions>(configuration.GetSection(TRexOptions.SectionName));
        services.AddScoped<IIdentityRegistryContractService, IdentityRegistryContractService>();
        services.AddScoped<IRealEstateRegistryContractService, RealEstateRegistryContractService>();
        services.AddScoped<IERC20ContractService, ERC20ContractService>();
        services.AddScoped<IRealEstateMarketplaceContractService, RealEstateMarketplaceContractService>();
        services.AddScoped<IComplianceTokenContractService, ComplianceTokenContractService>();
        services.AddScoped<ITREXFactoryContractService, TREXFactoryContractService>();
        services.AddScoped<IRealEstateVaultFactoryContractService, RealEstateVaultFactoryContractService>();
        services.AddScoped<ITokenReaderContractService, TokenReaderContractService>();
        services.AddScoped<IModularComplianceContractService, ModularComplianceContractService>();
        services.AddSingleton<IKycClaimTopicProvider, KycClaimTopicProvider>();
        services.AddSingleton<IBlockchainSettings, BlockchainSettings>();

        services.AddScoped<IOnChainKycActionRepository, OnChainKycActionRepository>();
        services.AddScoped<IOnChainVaultSupplyRepository, OnChainVaultSupplyRepository>();
        services.AddScoped<IOnChainPropertyRegistrationRepository, OnChainPropertyRegistrationRepository>();
        services.AddScoped<IPropertyActivationRecordRepository, PropertyActivationRecordRepository>();
        services.AddScoped<IOnChainSharePurchaseRepository, OnChainSharePurchaseRepository>();
        services.AddScoped<IOnChainShareSaleRepository, OnChainShareSaleRepository>();

        return services;
    }
}
