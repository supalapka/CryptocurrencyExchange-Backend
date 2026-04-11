using CryptocurrencyExchange.Core.Interfaces;
using CryptocurrencyExchange.Core.Interfaces.Repositories;
using CryptocurrencyExchange.Core.Interfaces.Services;
using CryptocurrencyExchange.Infrastructure.Market;
using CryptocurrencyExchange.Infrastructure.News;
using CryptocurrencyExchange.Infrastructure.Persistence;
using CryptocurrencyExchange.Infrastructure.Persistence.Repositories;
using CryptocurrencyExchange.Infrastructure.Schedulers;
using CryptocurrencyExchange.Infrastructure.Wallets;
using CryptocurrencyExchange.Options;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CryptocurrencyExchange.Extensions
{
    public static class InfrastructureCollectionExtensions
    {
        public static IServiceCollection AddPersistenceInfrastructureServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    o => o.CommandTimeout(300));
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

        public static IServiceCollection AddMessagingInfrastructure(this IServiceCollection services, IConfiguration configuration)
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

        public static IServiceCollection AddOutputCaching(this IServiceCollection services)
        {
            services.AddOutputCache();
            return services;
        }

        public static IServiceCollection AddBackgroundJobs(this IServiceCollection services)
        {
            services.AddHostedService<StakingScheduler>();
            return services;
        }

        public static IServiceCollection AddStakingPromotionOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddOptions<StakingPromotionOptions>()
                .Bind(configuration.GetSection("StakingPromotion"))
                .Validate(o => o.MinimumUsdtBalance > 0, "MinimumUsdtBalance must be positive")
                .ValidateOnStart();

            return services;
        }
    }
}
