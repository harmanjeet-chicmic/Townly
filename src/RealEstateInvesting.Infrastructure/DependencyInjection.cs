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
using RealEstateInvesting.Application.Health.Queries;
using RealEstateInvesting.Infrastructure.Blockchain;
using RealEstateInvesting.Infrastructure.Health;
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

        // HTTP context (required for CurrentUser)


        // Auth services
        services.AddScoped<IWalletNonceService, WalletNonceService>();
        services.AddScoped<IWalletAuthService, WalletAuthService>();
        services.AddScoped<IJwtService, JwtService>();

        // Identity / Current user
        services.AddScoped<ICurrentUser, CurrentUser>();

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IKycRecordRepository, KycRecordRepository>();
        services.AddScoped<SubmitKycHandler>();
        services.AddScoped<IKycFileStorageService, KycFileStorageService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddHttpClient<IPriceFeed, CoinGeckoPriceFeed>();
        services.AddScoped<IAdminKycService, AdminKycService>();

        services.AddScoped<IAdminKycRepository, AdminKycRepository>();
        services.AddScoped<IAdminKycService, AdminKycService>();
        services.AddScoped<IGetMyKycStatusReadService, KycStatusReadService>();
        services.AddScoped<IHealthCheckService, HealthCheckService>();
        services.AddScoped<ILogRepository, LogRepository>();

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
        services.AddScoped<IOnChainSharePurchaseRepository, OnChainSharePurchaseRepository>();
        services.AddScoped<IOnChainShareSaleRepository, OnChainShareSaleRepository>();

        return services;
    }
}
