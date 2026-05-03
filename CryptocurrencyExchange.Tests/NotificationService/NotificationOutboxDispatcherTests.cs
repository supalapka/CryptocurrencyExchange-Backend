using CryptocurrencyExchange.Core.Events;
using CryptocurrencyExchange.NotificationService.Entities;
using CryptocurrencyExchange.NotificationService.Interfaces;
using CryptocurrencyExchange.NotificationService.Outbox;
using CryptocurrencyExchange.NotificationService.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

namespace CryptocurrencyExchange.Tests.NotificationService
{
    [TestFixture]
    public class NotificationOutboxDispatcherTests
    {
        private Mock<INotificationOutboxRepository> _outboxRepo;
        private Mock<IPublishEndpoint> _publishEndpoint;
        private NotificationDbContext _dbContext;
        private NotificationOutboxDispatcher _dispatcher;

        [SetUp]
        public void SetUp()
        {
            _outboxRepo = new Mock<INotificationOutboxRepository>();
            _publishEndpoint = new Mock<IPublishEndpoint>();

            var options = new DbContextOptionsBuilder<NotificationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _dbContext = new NotificationDbContext(options);

            var services = new ServiceCollection();
            services.AddSingleton(_outboxRepo.Object);
            services.AddSingleton(_publishEndpoint.Object);
            services.AddSingleton(_dbContext);

            var scopeFactory = services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>();
            _dispatcher = new NotificationOutboxDispatcher(scopeFactory, NullLogger<NotificationOutboxDispatcher>.Instance);
        }

        [TearDown]
        public void TearDown() => _dbContext.Dispose();

        [Test]
        public async Task DispatchPendingAsync_WhenNoPendingEntries_DoesNotPublishOrSave()
        {
            _outboxRepo.Setup(x => x.GetPendingAsync()).ReturnsAsync(new List<NotificationOutbox>());

            await _dispatcher.DispatchPendingAsync();

            _publishEndpoint.Verify(
                x => x.Publish(It.IsAny<SendTransferNotificationEmailCommand>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Test]
        public async Task DispatchPendingAsync_WhenPendingEntriesExist_PublishesCommand()
        {
            var entry = new NotificationOutbox("user@test.com", "Transfer completed", "Your transfer is done.");
            _outboxRepo.Setup(x => x.GetPendingAsync()).ReturnsAsync(new List<NotificationOutbox> { entry });

            await _dispatcher.DispatchPendingAsync();

            _publishEndpoint.Verify(
                x => x.Publish(
                    It.Is<SendTransferNotificationEmailCommand>(c =>
                        c.Email == "user@test.com" &&
                        c.Subject == "Transfer completed" &&
                        c.Body == "Your transfer is done."),
                    It.IsAny<CancellationToken>()),
                Times.Once);

        }

        [Test]
        public async Task DispatchPendingAsync_WhenPendingEntriesExist_MarksEntriesAsProcessed()
        {
            var entry = new NotificationOutbox("user@test.com", "Transfer completed", "Your transfer is done.");
            _outboxRepo.Setup(x => x.GetPendingAsync()).ReturnsAsync(new List<NotificationOutbox> { entry });

            await _dispatcher.DispatchPendingAsync();

            Assert.That(entry.ProcessedAt, Is.Not.Null);
        }
    }
}
