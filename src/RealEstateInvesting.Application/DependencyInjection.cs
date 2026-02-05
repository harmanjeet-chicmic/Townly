using Microsoft.Extensions.DependencyInjection;
using RealEstateInvesting.Application.Properties;
using RealEstateInvesting.Application.Investments;
using RealEstateInvesting.Application.Transactions;
using RealEstateInvesting.Application.Analytics;
using RealEstateInvesting.Application.Portfolio;
using RealEstateInvesting.Application.Notifications;
using RealEstateInvesting.Application.Health.Handlers;
using RealEstateInvesting.Application.Properties.InvestmentInfo;
namespace RealEstateInvesting.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        // -------------------------------
        // Properties
        // -------------------------------
        services.AddScoped<PropertyService>();
        services.AddScoped<PropertyQueryService>();
        services.AddScoped<PropertyUpdateService>();

        // -------------------------------
        // Investments / Transactions
        // -------------------------------
        services.AddScoped<InvestmentService>();
        services.AddScoped<InvestmentQueryService>();
        services.AddScoped<TransactionQueryService>();

        // -------------------------------
        // Analytics / Portfolio
        // -------------------------------
        services.AddScoped<AnalyticsQueryService>();
        services.AddScoped<PortfolioQueryService>();

        // -------------------------------
        // Notifications
        // -------------------------------
        services.AddScoped<NotificationService>();

        services.AddScoped<GetHealthStatusHandler>();
        services.AddScoped<PropertyInvestmentInfoService>();

        return services;
    }
}
