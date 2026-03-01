using CryptocurrencyExchange.Core.Interfaces;
using CryptocurrencyExchange.Core.Interfaces.Repositories;
using CryptocurrencyExchange.Core.Interfaces.Services;
using CryptocurrencyExchange.Infrastructure.Logging;
using CryptocurrencyExchange.Infrastructure.Market;
using CryptocurrencyExchange.Infrastructure.News;
using CryptocurrencyExchange.Infrastructure.Persistence;
using CryptocurrencyExchange.Infrastructure.Persistence.Repositories;
using CryptocurrencyExchange.Infrastructure.Schedulers;
using CryptocurrencyExchange.Infrastructure.Security;
using CryptocurrencyExchange.Options;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CryptocurrencyExchange.Extensions
{
    public static class InfrastructureCollectionExtensions
    {
        public static IServiceCollection AddPersistenceInfrastructureServices(
             this IServiceCollection services,
             IConfiguration configuration)
        {
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<IUnitOfWork, EfUniOfWork>();

            services.AddScoped<IStakingRepository, EfStakingRepository>();
            services.AddScoped<IWalletItemRepository, WalletItemRepository>();
            services.AddScoped<IFutureRepository, EfFutureRepository>();
            services.AddScoped<ICryptoNewsRepository, NewPersistence>();
            services.AddScoped<IUserRepository, EfUserRepository>();

            return services;
        }

        public static IServiceCollection AddExternalApiInfrastructureServices(this IServiceCollection services)
        {
            services.AddHttpClient<IMarketApiClient, BinanceMarketApiClient>(client =>
            {
                client.BaseAddress = new Uri("https://api.binance.com/api/v3/");
            });

            services.AddScoped<IMarketPriceProvider, ApiMarketPriceProvider>();
            services.AddScoped<ICryptoNewsUpdateRequester, CryptoNewsCrawlRequester>();

            return services;
        }

        public static IServiceCollection AddMessagingInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            services
                .AddOptions<RabbitMqOptions>()
                .Bind(configuration.GetSection("RabbitMq"))
                .Validate(o => !string.IsNullOrWhiteSpace(o.Host))
                .Validate(o => !string.IsNullOrWhiteSpace(o.Username))
                .Validate(o => !string.IsNullOrWhiteSpace(o.Password))
                .ValidateOnStart();

            services.AddMassTransit(x =>
            {
                x.AddConsumer<CryptoNewsConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    var options = context
                        .GetRequiredService<IOptions<RabbitMqOptions>>()
                        .Value;

                    cfg.Host(options.Host, options.VirtualHost, h =>
                    {
                        h.Username(options.Username);
                        h.Password(options.Password);
                    });

                    cfg.ReceiveEndpoint("news.url-matched", e =>
                    {
                        e.ConfigureConsumer<CryptoNewsConsumer>(context);
                    });
                });
            });

            services.AddMassTransitHostedService();

            return services;
        }

        public static IServiceCollection AddSecurityInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            services
                .AddOptions<JwtOptions>()
                .Bind(configuration.GetSection("Jwt"))
                .Validate(o => o.SecretKey.Length >= 32, "JWT key is too short")
                .ValidateOnStart();

            var jwtOptions = configuration
                .GetSection("Jwt")
                .Get<JwtOptions>()
                ?? throw new InvalidOperationException("Jwt configuration is missing");

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddScoped<ITokenService, JwtTokenService>();

            return services;
        }

        public static IServiceCollection AddBackgroundJobs(
        this IServiceCollection services)
        {
            services.AddHostedService<StakingScheduler>();
            return services;
        }

        public static IServiceCollection AddDatabaseLogging(this IServiceCollection services)
        {
            services.AddSingleton<LogQueue>();
            services.AddSingleton<ILoggerProvider, DatabaseLoggerProvider>();
            services.AddHostedService<DatabaseLogWriterService>();
            return services;
        }
    }
}