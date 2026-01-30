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


        return services;
    }
}
