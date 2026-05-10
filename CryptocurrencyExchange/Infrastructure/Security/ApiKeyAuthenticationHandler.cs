using CryptocurrencyExchange.Core.Interfaces.Repositories;
using CryptocurrencyExchange.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;

namespace CryptocurrencyExchange.Infrastructure.Security
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        private readonly IApiKeyRepository _apiKeyRepository;

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<ApiKeyAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IApiKeyRepository apiKeyRepository)
            : base(options, logger, encoder, clock)
        {
            _apiKeyRepository = apiKeyRepository;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("X-API-Key", out var keyValues) || string.IsNullOrEmpty(keyValues))
                return AuthenticateResult.NoResult();

            var rawKey = keyValues.ToString();
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(rawKey));
            var apiKey = await _apiKeyRepository.GetByHashAsync(hash);

            if (apiKey == null)
                return AuthenticateResult.Fail("Invalid API key");

            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, apiKey.UserId.ToString()) };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
        }
    }
}
