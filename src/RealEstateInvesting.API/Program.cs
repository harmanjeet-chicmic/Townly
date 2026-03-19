using Amazon;
using Amazon.S3;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using RealEstateInvesting.Infrastructure.Organizations;
using RealEstateInvesting.API.Filters;
using RealEstateInvesting.API.RequestDebugMiddleware;
using RealEstateInvesting.Application;
using RealEstateInvesting.Application.Admin.Properties;
using RealEstateInvesting.Application.Admin.Properties.Interfaces;
using RealEstateInvesting.Application.Admin.Users;
using RealEstateInvesting.Application.Admin.Users.Interfaces;
using RealEstateInvesting.Application.AdminAuth;
using RealEstateInvesting.Application.AdminAuth.Interfaces;
using RealEstateInvesting.Application.Analytics;
using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Application.Investments;
using RealEstateInvesting.Application.Kyc.Handlers;
using RealEstateInvesting.Application.Notifications;
using RealEstateInvesting.Application.Notifications.Interfaces;
using RealEstateInvesting.Application.Portfolio;
using RealEstateInvesting.Application.Properties;
using RealEstateInvesting.Application.Tokens.Balance;
using RealEstateInvesting.Application.Tokens.Requests;
using RealEstateInvesting.Application.Transactions;
using RealEstateInvesting.Infrastructure;
using RealEstateInvesting.Infrastructure.Admin.Properties;
using RealEstateInvesting.Infrastructure.Admin.Users;
using RealEstateInvesting.Infrastructure.BackgroundJobs;
using RealEstateInvesting.Infrastructure.Persistence.Repositories;
using RealEstateInvesting.Infrastructure.Pricing;
using RealEstateInvesting.Infrastructure.Push;
using RealEstateInvesting.Infrastructure.Security;
using RealEstateInvesting.Infrastructure.Storage;
using RealEstateInvesting.Infrastructure.Push;
using RealEstateInvesting.Infrastructure.Pricing;
using RealEstateInvesting.Infrastructure.VectorSearch;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Memory;
using RealEstateInvesting.Application.Portfolio;
using RealEstateInvesting.Application.AdminAuth;
using RealEstateInvesting.Application.AdminAuth.Interfaces;
using RealEstateInvesting.Api.Middleware;
using RealEstateInvesting.Infrastructure.Security;
using Serilog;
using System.Text;
using RealEstateInvesting.Application.Notifications.Interfaces;
using RealEstateInvesting.Application.Notifications;
using RealEstateInvesting.Application.Kyc.Handlers;
using RealEstateInvesting.Application.Admin.Kyc.Interfaces;
using RealEstateInvesting.Application.Admin.Kyc;
using RealEstateInvesting.Infrastructure.Admin.Kyc;
using RealEstateInvesting.Application;
using RealEstateInvesting.API.RequestDebugMiddleware;
using RealEstateInvesting.Application.Tokens.Requests;
using RealEstateInvesting.Application.Tokens.Balance;
using Microsoft.AspNetCore.RateLimiting;
using RealEstateInvesting.Application.Admin.Users;
using RealEstateInvesting.Application.Admin.Users.Interfaces;
using RealEstateInvesting.Infrastructure.Admin.Users;

using System.Threading.RateLimiting;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);
var firebasePath = builder.Configuration["Firebase:ServiceAccountPath"];

if (!string.IsNullOrWhiteSpace(firebasePath))
{
    firebasePath = Path.Combine(
        builder.Environment.ContentRootPath,
        firebasePath
    );
}
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/webapi-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

// Replace default logging with Serilog
//builder.Host.UseSerilog();
FirebaseInitializer.Initialize(firebasePath);

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

builder.Services.AddScoped<IAdminUserService, AdminUserService>();
builder.Services.AddScoped<IAdminUserRepository, AdminUserRepository>();
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
builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();
builder.Services.AddScoped<OrganizationQueryService>();
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
builder.Services.AddVectorSearch(builder.Configuration);

builder.Services.AddHostedService<AnalyticsBackgroundService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme."
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("bearer", document)] = []
    });
});




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
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });

    options.AddPolicy("DevCorsPolicy", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowed(origin =>
            {
                // Allow local dev and common ports
                if (origin.StartsWith("http://localhost:") || 
                    origin.StartsWith("http://127.0.0.1:"))
                    return true;

                // Allow any ngrok HTTPS domain
                if (origin.StartsWith("https://") &&
                    origin.Contains("ngrok-free.dev"))
                    return true;

                return false;
            });
    });
});
// Required for CurrentUserService
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();


// builder.Services.AddVectorSearch(builder.Configuration); // Removed duplicate

builder.Services.AddAuthorization();
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("PropertyCreationPolicy", context =>
    {
        var userId = context.User?.FindFirst(
            System.Security.Claims.ClaimTypes.NameIdentifier
        )?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return RateLimitPartition.GetNoLimiter("anonymous");
        }

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: userId,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromHours(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            });
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// Infrastructure already added above

var app = builder.Build();
app.UseMiddleware<RealEstateInvesting.API.Middleware.ExceptionMiddleware>();

app.UseCors("AllowAll");
if (app.Environment.IsDevelopment())
{

}
app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();
app.UseStaticFiles();

app.UseHttpsRedirection();
// app.UseCors("DevCorsPolicy"); // Removed redundant and misordered call
app.UseMiddleware<RequestDebugMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
//app.UseMiddleware<RequestResponseLoggingMiddleware>();
app.MapControllers();

app.Run();
