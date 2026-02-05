using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RealEstateInvesting.API.Filters;
using RealEstateInvesting.Infrastructure;
using RealEstateInvesting.Application.Properties;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Infrastructure.Persistence.Repositories;
using RealEstateInvesting.Application.Investments;
using RealEstateInvesting.Application.Transactions;
using RealEstateInvesting.Infrastructure.BackgroundJobs;
using RealEstateInvesting.Application.Analytics;
using Amazon;
using Amazon.S3;
using RealEstateInvesting.Infrastructure.Storage;

using RealEstateInvesting.Infrastructure.Pricing;
using Microsoft.Extensions.Caching.Memory;
using RealEstateInvesting.Application.Portfolio;
using RealEstateInvesting.Infrastructure.VectorSearch;
using RealEstateInvesting.Application.AdminAuth;
using RealEstateInvesting.Application.AdminAuth.Interfaces;

using RealEstateInvesting.Infrastructure.Security;
using Microsoft.AspNetCore.Identity;
using RealEstateInvesting.Domain.Entities;
using RealEstateInvesting.Application.Admin.Properties;
using RealEstateInvesting.Application.Admin.Properties.Interfaces;
using RealEstateInvesting.Infrastructure.Admin.Properties;
using System.Text;
using RealEstateInvesting.Application.Notifications.Interfaces;
using RealEstateInvesting.Application.Notifications;
using RealEstateInvesting.Application.Kyc.Handlers;
using RealEstateInvesting.Application;
using RealEstateInvesting.API.RequestDebugMiddleware;
using RealEstateInvesting.Application.Tokens.Requests;
using RealEstateInvesting.Application.Tokens.Balance;

var builder = WebApplication.CreateBuilder(args);
// var hasher = new PasswordHasher<AdminUser>();
// var hash = hasher.HashPassword(null!, "Admin@123");
// Console.WriteLine("============================================================");
// Console.WriteLine(hash);
// Console.WriteLine("======================================");
builder.Services.AddOpenApi();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidateModelFilter>();
});
//---------------------------------------------------

// -------------------------------
// AWS S3 Configuration
// -------------------------------
var awsSection = builder.Configuration.GetSection("AWS");
builder.Services.AddScoped<PortfolioQueryService>();
builder.Services.AddScoped<IAdminPropertyService, AdminPropertyService>();
builder.Services.AddScoped<IAdminPropertyRepository, AdminPropertyRepository>();
builder.Services.AddScoped<GetMyKycStatusHandler>();

builder.Services.AddSingleton<IAmazonS3>(_ =>
{
    return new AmazonS3Client(
        awsSection["AccessKey"],
        awsSection["SecretKey"],
        RegionEndpoint.GetBySystemName(awsSection["Region"])
    );
});
builder.Services.AddScoped<PortfolioQueryService>();

// -------------------------------
// File Storage (Local / S3)
// -------------------------------
builder.Services.AddScoped<IFileStorage>(sp =>
{
    return new S3FileStorage(
        sp.GetRequiredService<IAmazonS3>(),
        awsSection["BucketName"]!,
        awsSection["BasePrefix"]!,
        awsSection["Region"]!
    );
});
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//----------------------------------------------------------------
// 🔐 JWT 

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
        ),

        ClockSkew = TimeSpan.Zero
    };
     options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {   

            Console.WriteLine("=======jwt middleware hitted========");
            var authHeader = context.Request.Headers["Authorization"].ToString();

            Console.WriteLine("================== JWT RECEIVED ==========");
            Console.WriteLine(authHeader);

            return Task.CompletedTask;
        },

        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("========== JWT FAILED ==========");
            Console.WriteLine(context.Exception.Message);

            return Task.CompletedTask;
        }
    };
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCorsPolicy", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowed(origin =>
            {
                // Allow local dev
                if (origin == "http://127.0.0.1:5500" ||
                    origin == "http://localhost:5500")
                    return true;

                // Allow any ngrok HTTPS domain
                if (origin.StartsWith("https://") &&
                    origin.Contains("ngrok-free.dev"))
                    return true;

                return false;
            });
    });
});
// Application services
builder.Services.AddScoped<PropertyService>();
builder.Services.AddScoped<PropertyUpdateService>();
builder.Services.AddScoped<PropertyQueryService>();
builder.Services.AddScoped<IInvestmentRepository, InvestmentRepository>();
builder.Services.AddScoped<InvestmentService>();

builder.Services.AddScoped<InvestmentQueryService>();
builder.Services.AddScoped<TransactionQueryService>();

// Repositories
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPropertyDocumentRepository, PropertyDocumentRepository>();
builder.Services.AddScoped<IPropertyUpdateRequestRepository, PropertyUpdateRequestRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

builder.Services.AddScoped<IAnalyticsSnapshotRepository, AnalyticsSnapshotRepository>();
builder.Services.AddScoped<AnalyticsQueryService>();
builder.Services.AddHostedService<AnalyticsBackgroundService>();
builder.Services.AddMemoryCache();

builder.Services.AddHttpClient<CoinGeckoEthPriceService>();
builder.Services.AddVectorSearch(builder.Configuration);

builder.Services.AddScoped<IEthPriceService>(sp =>
{
    var live = sp.GetRequiredService<CoinGeckoEthPriceService>();
    var cache = sp.GetRequiredService<IMemoryCache>();

    return new CachedEthPriceService(live, cache);
});
// Admin auth
builder.Services.AddScoped<IAdminAuthService, AdminAuthService>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IAdminPasswordHasher, AdminPasswordHasher>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

// Required for CurrentUserService
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Token system repositories
builder.Services.AddScoped<ITokenRequestRepository, TokenRequestRepository>();
builder.Services.AddScoped<IUserTokenBalanceRepository, UserTokenBalanceRepository>();
builder.Services.AddScoped<ITokenTransactionRepository, TokenTransactionRepository>();

// Token system handlers/services
builder.Services.AddScoped<CreateTokenRequestHandler>();
builder.Services.AddScoped<ReviewTokenRequestHandler>();
builder.Services.AddScoped<UserTokenBalanceService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
builder.Services.AddVectorSearch(builder.Configuration);

builder.Services.AddAuthorization();

// Infrastructure (DbContext, services, etc.)
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();
app.UseCors("AllowAll");
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseCors("DevCorsPolicy");
app.UseMiddleware<RequestDebugMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
