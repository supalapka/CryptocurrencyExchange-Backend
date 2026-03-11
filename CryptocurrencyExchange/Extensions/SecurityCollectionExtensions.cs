using CryptocurrencyExchange.Core.Interfaces;
using CryptocurrencyExchange.Infrastructure.Security;
using CryptocurrencyExchange.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;

namespace CryptocurrencyExchange.Extensions
{
    public static class SecurityCollectionExtensions
    {
        public static IServiceCollection AddSecurityInfrastructure(this IServiceCollection services, IConfiguration configuration)
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

        public static IServiceCollection AddRateLimiting(this IServiceCollection services, IConfiguration configuration)
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
    }
}
