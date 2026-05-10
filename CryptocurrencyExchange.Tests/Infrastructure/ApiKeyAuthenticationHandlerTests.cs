using CryptocurrencyExchange.Core.Interfaces.Repositories;
using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Infrastructure.Security;
using CryptocurrencyExchange.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace CryptocurrencyExchange.Tests.Infrastructure
{
    [TestFixture]
    public class ApiKeyAuthenticationHandlerTests
    {
        private Mock<IApiKeyRepository> _apiKeyRepo;
        private Mock<IOptionsMonitor<ApiKeyAuthenticationOptions>> _options;

        [SetUp]
        public void SetUp()
        {
            _apiKeyRepo = new Mock<IApiKeyRepository>();
            _options = new Mock<IOptionsMonitor<ApiKeyAuthenticationOptions>>();
            _options.Setup(x => x.Get(It.IsAny<string>())).Returns(new ApiKeyAuthenticationOptions());
        }

        private async Task<(ApiKeyAuthenticationHandler handler, DefaultHttpContext context)> CreateHandlerAsync()
        {
            var context = new DefaultHttpContext();
            var handler = new ApiKeyAuthenticationHandler(
                _options.Object,
                NullLoggerFactory.Instance,
                UrlEncoder.Default,
                new SystemClock(),
                _apiKeyRepo.Object);

            var scheme = new AuthenticationScheme("ApiKey", null, typeof(ApiKeyAuthenticationHandler));
            await handler.InitializeAsync(scheme, context);

            return (handler, context);
        }

        [Test]
        public async Task HandleAuthenticate_WhenHeaderMissing_ReturnsNoResult()
        {
            var (handler, _) = await CreateHandlerAsync();

            var result = await handler.AuthenticateAsync();

            Assert.That(result.None, Is.True);
        }

        [Test]
        public async Task HandleAuthenticate_WhenKeyNotFound_ReturnsFail()
        {
            var (handler, context) = await CreateHandlerAsync();
            context.Request.Headers["X-API-Key"] = "unknown-key";
            _apiKeyRepo.Setup(x => x.GetByHashAsync(It.IsAny<byte[]>())).ReturnsAsync((ApiKey)null);

            var result = await handler.AuthenticateAsync();

            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Failure!.Message, Is.EqualTo("Invalid API key"));
        }

        [Test]
        public async Task HandleAuthenticate_WhenKeyValid_ReturnsSuccessWithUserIdClaim()
        {
            var (handler, context) = await CreateHandlerAsync();
            var (apiKey, rawKey) = ApiKey.Create(42);
            context.Request.Headers["X-API-Key"] = rawKey;
            _apiKeyRepo.Setup(x => x.GetByHashAsync(It.IsAny<byte[]>())).ReturnsAsync(apiKey);

            var result = await handler.AuthenticateAsync();

            Assert.That(result.Succeeded, Is.True);
            var userId = result.Principal!.FindFirstValue(ClaimTypes.NameIdentifier);
            Assert.That(userId, Is.EqualTo("42"));
        }
    }
}
