using CryptocurrencyExchange.Core.Domain;
using CryptocurrencyExchange.Core.Interfaces;
using CryptocurrencyExchange.Core.Interfaces.Services;
using CryptocurrencyExchange.Application.Auth;
using CryptocurrencyExchange.Application.Futures;
using CryptocurrencyExchange.Application.Market;
using CryptocurrencyExchange.Application.Notifications;
using CryptocurrencyExchange.Application.StakingServices;
using CryptocurrencyExchange.Application.Wallets;
using CryptocurrencyExchange.Services.Interfaces;

namespace CryptocurrencyExchange.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IMarketService, MarketService>();
            services.AddScoped<IStakingService, StakingService>();
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<IFuturesService, FuturesService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<INotificationService, NotificationService>();

            services.AddScoped<IStakingDomainService, StakingDomainService>();
            services.AddScoped<IFuturesDomainService, FuturesDomainService>();
            services.AddScoped<IAuthDomainService, AuthDomainService>();

            return services;
        }

        public static IServiceCollection AddWebServices(
        this IServiceCollection services)
        {
            services.AddControllers();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            return services;
        }
    }
}
