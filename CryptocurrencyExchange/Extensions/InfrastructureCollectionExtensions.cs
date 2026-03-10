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
using CryptocurrencyExchange.Infrastructure.Wallets;
using CryptocurrencyExchange.Options;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System.Text;
using System.Threading.RateLimiting;

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
            services.AddScoped<ITransferRepository, EfTransferRepository>();

            return services;
        }

        public static IServiceCollection AddExternalApiInfrastructureServices(this IServiceCollection services)
        {
            services.AddHttpClient<IMarketApiClient, BinanceMarketApiClient>();
            services.AddHttpClient<IMarketApiClient, BybitMarketApiClient>();
            services.AddSingleton<IMarketPriceProvider, RoutingApiMarketPriceProvider>();

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
                x.AddConsumer<StarterWalletConsumer>();

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

                    cfg.ConfigureEndpoints(context);
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

        public static IServiceCollection AddRateLimiting(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            services
                .AddOptions<RateLimitingOptions>()
                .Bind(configuration.GetSection("RateLimiting"))
                .Validate(o => o.PermitLimit > 0, "PermitLimit must be greater than 0")
                .Validate(o => o.WindowSeconds > 0, "WindowSeconds must be greater than 0")
                .Validate(o => o.QueueLimit >= 0, "QueueLimit must not be negative")
                .ValidateOnStart();

            var options = configuration
                .GetSection("RateLimiting")
                .Get<RateLimitingOptions>()
                ?? throw new InvalidOperationException("RateLimiting configuration is missing");

            services.AddRateLimiter(limiter =>
            {
                limiter.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                limiter.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = options.PermitLimit,
                            Window = TimeSpan.FromSeconds(options.WindowSeconds),
                            QueueLimit = options.QueueLimit,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                        }));
            });

            return services;
        }

        public static IServiceCollection AddOutputCaching(this IServiceCollection services)
        {
            services.AddOutputCache();
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

        public static IHostBuilder AddElasticLogging(
            this IHostBuilder hostBuilder,
            IConfiguration configuration)
        {
            hostBuilder.ConfigureServices((_, services) =>
            {
                services
                    .AddOptions<ElasticsearchOptions>()
                    .Bind(configuration.GetSection("Elasticsearch"))
                    .Validate(o => !string.IsNullOrWhiteSpace(o.Uri),
                              "Elasticsearch:Uri is required")
                    .Validate(o => Uri.TryCreate(o.Uri, UriKind.Absolute, out var uri),
                              "Elasticsearch:Uri must be a valid absolute URI")
                    .ValidateOnStart();
            });

            hostBuilder.UseSerilog(
                (hostContext, _, loggerConfig) =>
                {
                    var elasticOptions = hostContext.Configuration
                        .GetSection("Elasticsearch")
                        .Get<ElasticsearchOptions>()
                        ?? throw new InvalidOperationException(
                            "Elasticsearch configuration section is missing");

                    loggerConfig
                        .ReadFrom.Configuration(hostContext.Configuration)
                        .Enrich.FromLogContext()
                        .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(
                            new Uri(elasticOptions.Uri))
                        {
                            IndexFormat = elasticOptions.IndexFormat,
                            AutoRegisterTemplate = true,
                            AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv8,
                            NumberOfShards = 1,
                            NumberOfReplicas = 0,
                            ModifyConnectionSettings = conn =>
                            {
                                if (!string.IsNullOrWhiteSpace(elasticOptions.Username))
                                    conn.BasicAuthentication(
                                        elasticOptions.Username,
                                        elasticOptions.Password);
                                return conn;
                            }
                        });
                },
                writeToProviders: true);

            return hostBuilder;
        }
    }
}