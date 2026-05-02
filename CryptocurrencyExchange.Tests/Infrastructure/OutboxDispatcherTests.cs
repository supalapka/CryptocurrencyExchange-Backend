using CryptocurrencyExchange.Core.Events;
using CryptocurrencyExchange.Core.Interfaces;
using CryptocurrencyExchange.Core.Interfaces.Repositories;
using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Infrastructure.Outbox;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

namespace CryptocurrencyExchange.Tests.Infrastructure
{
    [TestFixture]
    public class OutboxDispatcherTests
    {
        private Mock<IUserRegistrationOutboxRepository> _outboxRepo;
        private Mock<IPublishEndpoint> _publishEndpoint;
        private Mock<IUnitOfWork> _uow;
        private OutboxDispatcher _dispatcher;

        [SetUp]
        public void SetUp()
        {
            _outboxRepo = new Mock<IUserRegistrationOutboxRepository>();
            _publishEndpoint = new Mock<IPublishEndpoint>();
            _uow = new Mock<IUnitOfWork>();

            var services = new ServiceCollection();
            services.AddSingleton(_outboxRepo.Object);
            services.AddSingleton(_publishEndpoint.Object);
            services.AddSingleton(_uow.Object);

            var scopeFactory = services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>();
            _dispatcher = new OutboxDispatcher(scopeFactory, NullLogger<OutboxDispatcher>.Instance);
        }

        [Test]
        public async Task DispatchPendingAsync_WhenNoPendingEntries_DoesNotPublishOrCommit()
        {
            _outboxRepo.Setup(x => x.GetPendingAsync()).ReturnsAsync(new List<UserRegistrationOutbox>());

            await _dispatcher.DispatchPendingAsync();

            _publishEndpoint.Verify(
                x => x.Publish(It.IsAny<UserRegisteredEvent>(), It.IsAny<CancellationToken>()),
                Times.Never);
            _uow.Verify(x => x.CommitAsync(), Times.Never);
        }

        [Test]
        public async Task DispatchPendingAsync_WhenPendingEntriesExist_PublishesAndCommits()
        {
            var entry = new UserRegistrationOutbox(42, "user@example.com");
            _outboxRepo.Setup(x => x.GetPendingAsync()).ReturnsAsync(new List<UserRegistrationOutbox> { entry });

            await _dispatcher.DispatchPendingAsync();

            _publishEndpoint.Verify(
                x => x.Publish(
                    It.Is<UserRegisteredEvent>(e => e.UserId == 42 && e.Email == "user@example.com"),
                    It.IsAny<CancellationToken>()),
                Times.Once);
            _uow.Verify(x => x.CommitAsync(), Times.Once);
        }

        [Test]
        public async Task DispatchPendingAsync_WhenPendingEntriesExist_MarksEntriesAsProcessed()
        {
            var entry = new UserRegistrationOutbox(42, "user@example.com");
            _outboxRepo.Setup(x => x.GetPendingAsync()).ReturnsAsync(new List<UserRegistrationOutbox> { entry });

            await _dispatcher.DispatchPendingAsync();

            Assert.That(entry.ProcessedAt, Is.Not.Null);
        }
    }
}
