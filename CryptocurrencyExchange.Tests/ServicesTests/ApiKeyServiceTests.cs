using CryptocurrencyExchange.Application.ApiKeys;
using CryptocurrencyExchange.Core.Interfaces;
using CryptocurrencyExchange.Core.Interfaces.Repositories;
using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Exceptions;
using Moq;
using NUnit.Framework;

namespace CryptocurrencyExchange.Tests.ServicesTests
{
    [TestFixture]
    public class ApiKeyServiceTests
    {
        private Mock<IApiKeyRepository> _apiKeyRepo;
        private Mock<IUnitOfWork> _uow;
        private ApiKeyService _service;

        [SetUp]
        public void SetUp()
        {
            _apiKeyRepo = new Mock<IApiKeyRepository>();
            _uow = new Mock<IUnitOfWork>();
            _service = new ApiKeyService(_apiKeyRepo.Object, _uow.Object);
        }

        [Test]
        public async Task GenerateAsync_WhenNoExistingKey_AddsKeyCommitsAndReturnsRawKey()
        {
            _apiKeyRepo.Setup(x => x.GetByUserIdAsync(1)).ReturnsAsync((ApiKey)null);

            var rawKey = await _service.GenerateAsync(1);

            Assert.That(rawKey, Is.Not.Null.And.Not.Empty);
            _apiKeyRepo.Verify(x => x.DeleteAsync(It.IsAny<ApiKey>()), Times.Never);
            _apiKeyRepo.Verify(x => x.AddAsync(It.IsAny<ApiKey>()), Times.Once);
            _uow.Verify(x => x.CommitAsync(), Times.Once);
        }

        [Test]
        public async Task GenerateAsync_WhenExistingKey_DeletesOldThenAddsNew()
        {
            var (existing, _) = ApiKey.Create(1);
            _apiKeyRepo.Setup(x => x.GetByUserIdAsync(1)).ReturnsAsync(existing);

            var callOrder = new List<string>();
            _apiKeyRepo.Setup(x => x.DeleteAsync(existing))
                .Callback(() => callOrder.Add("delete"))
                .Returns(Task.CompletedTask);
            _apiKeyRepo.Setup(x => x.AddAsync(It.IsAny<ApiKey>()))
                .Callback(() => callOrder.Add("add"))
                .Returns(Task.CompletedTask);

            await _service.GenerateAsync(1);

            Assert.That(callOrder, Is.EqualTo(new[] { "delete", "add" }));
        }

        [Test]
        public async Task GetAsync_WhenKeyExists_ReturnsDtoWithPrefixAndCreatedAt()
        {
            var (apiKey, _) = ApiKey.Create(1);
            _apiKeyRepo.Setup(x => x.GetByUserIdAsync(1)).ReturnsAsync(apiKey);

            var dto = await _service.GetAsync(1);

            Assert.That(dto.Prefix, Is.EqualTo(apiKey.KeyPrefix));
            Assert.That(dto.CreatedAt, Is.EqualTo(apiKey.CreatedAt));
        }

        [Test]
        public void GetAsync_WhenNoKey_ThrowsApiKeyNotFoundException()
        {
            _apiKeyRepo.Setup(x => x.GetByUserIdAsync(1)).ReturnsAsync((ApiKey)null);

            Assert.ThrowsAsync<ApiKeyNotFoundException>(async () =>
                await _service.GetAsync(1));
        }
    }
}
