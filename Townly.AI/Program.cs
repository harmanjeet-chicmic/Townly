using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using Townly.AI.Infrastructure.HttpClients;
using Townly.AI.Services;
using Townly.AI.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ========================================
// 1️⃣ Controllers + Swagger
// ========================================

builder.Services.AddControllers();
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

// ========================================
// 2️⃣ Read JWT Configuration
// ========================================

var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrWhiteSpace(jwtKey))
    throw new Exception("JWT Key not configured in appsettings.json");

if (string.IsNullOrWhiteSpace(jwtIssuer))
    throw new Exception("JWT Issuer not configured in appsettings.json");

if (string.IsNullOrWhiteSpace(jwtAudience))
    throw new Exception("JWT Audience not configured in appsettings.json");

// ========================================
// 3️⃣ Authentication
// ========================================

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey))
        };

        // Optional debug logging
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("JWT AUTH FAILED:");
                Console.WriteLine(context.Exception.ToString());
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// ========================================
// 4️⃣ HttpClients
// ========================================

// Main Townly API client
builder.Services.AddHttpClient<TownlyApiClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["TownlyApi:BaseUrl"]!);
});

// Gemini client
builder.Services.AddHttpClient<ILlmClient, GeminiClient>();

builder.Services.AddHttpContextAccessor();

// ========================================
// 5️⃣ Application Services
// ========================================

builder.Services.AddScoped<IPortfolioAiService, PortfolioAiService>();
builder.Services.AddScoped<IPortfolioContextBuilder, PortfolioContextBuilder>();

// ========================================
// 6️⃣ CORS (Allow frontend)
// ========================================

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ========================================
// 7️⃣ Middleware Pipeline
// ========================================

if (app.Environment.IsDevelopment())
{
   
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();