using CryptocurrencyExchange.Core.Domain;
using CryptocurrencyExchange.Core.Interfaces;
using CryptocurrencyExchange.Core.Interfaces.Repositories;
using CryptocurrencyExchange.Core.Interfaces.Services;
using CryptocurrencyExchange.Data;
using CryptocurrencyExchange.Infrastructure.Market;
using CryptocurrencyExchange.Infrastructure.Schedulers;
using CryptocurrencyExchange.Infrastructure.Security;
using CryptocurrencyExchange.Middleware;
using CryptocurrencyExchange.Options;
using CryptocurrencyExchange.Services.Auth;
using CryptocurrencyExchange.Services.Futures;
using CryptocurrencyExchange.Services.Interfaces;
using CryptocurrencyExchange.Services.Market;
using CryptocurrencyExchange.Services.Notifications;
using CryptocurrencyExchange.Services.StakingServices;
using CryptocurrencyExchange.Services.Wallet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddOptions<JwtOptions>()
    .Bind(builder.Configuration.GetSection("Jwt"))
    .Validate(o => o.SecretKey.Length >= 32, "JWT key is too short")
    .ValidateOnStart();

builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddHttpClient<IMarketApiClient, BybitMarketApiClient>();
builder.Services.AddHttpClient<IMarketApiClient, BinanceMarketApiClient>();


builder.Services.AddScoped<IMarketService, MarketService>();
builder.Services.AddSingleton<IMarketPriceProvider, RoutingApiMarketPriceProvider>(); // carefull, do not inject Scoped services inside singleton

builder.Services.AddScoped<IStakingRepository, EfStakingRepository>();
builder.Services.AddScoped<IStakingDomainService, StakingDomainService>();
builder.Services.AddScoped<IStakingService, StakingService>();

builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IFuturesService, FuturesService>();

builder.Services.AddScoped<IWalletItemRepository, WalletItemRepository>();
builder.Services.AddScoped<IWalletDomainService, WalletDomainService>();

builder.Services.AddScoped<IUnitOfWork, EfUniOfWork>();

builder.Services.AddScoped<IFutureRepository, EfFutureRepository>();
builder.Services.AddScoped<IFuturesDomainService, FuturesDomainService>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthDomainService, AuthDomainService>();
builder.Services.AddScoped<IUserRepository, EfUserRepository>();

builder.Services.AddHostedService<StakingScheduler>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var jwtOptions = builder.Configuration
    .GetSection("Jwt")
    .Get<JwtOptions>()
    ?? throw new InvalidOperationException("Jwt configuration is missing");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtOptions.SecretKey)
            ),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseCors();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<JwtMiddleware>();

app.MapControllers();

app.Run();
