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
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Infrastructure.Pricing;
using Microsoft.Extensions.Caching.Memory;
using RealEstateInvesting.Application.Portfolio;
using RealEstateInvesting.Infrastructure.VectorSearch;


using System.Text;

var builder = WebApplication.CreateBuilder(args);

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
        awsSection["BasePrefix"]!);
});


//----------------------------------------------------------------
// 🔐 JWT Authentication
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

builder.Services.AddScoped<IEthPriceService>(sp =>
{
    var live = sp.GetRequiredService<CoinGeckoEthPriceService>();
    var cache = sp.GetRequiredService<IMemoryCache>();

    return new CachedEthPriceService(live, cache);
});


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
}
app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseCors("DevCorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
