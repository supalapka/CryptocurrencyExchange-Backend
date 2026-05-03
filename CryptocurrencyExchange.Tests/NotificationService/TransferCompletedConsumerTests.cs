using CryptocurrencyExchange.Core.Events;
using CryptocurrencyExchange.NotificationService.Consumers;
using CryptocurrencyExchange.NotificationService.Entities;
using CryptocurrencyExchange.NotificationService.Interfaces;
using CryptocurrencyExchange.NotificationService.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

namespace CryptocurrencyExchange.Tests.NotificationService
{
    [TestFixture]
    public class TransferCompletedConsumerTests
    {
        private Mock<IProcessedMessageRepository> _processedMessages;
        private Mock<INotificationOutboxRepository> _outboxRepo;
        private NotificationDbContext _dbContext;
        private TransferCompletedConsumer _consumer;

        [SetUp]
        public void SetUp()
        {
            _processedMessages = new Mock<IProcessedMessageRepository>();
            _outboxRepo = new Mock<INotificationOutboxRepository>();

            var options = new DbContextOptionsBuilder<NotificationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _dbContext = new NotificationDbContext(options);

            _consumer = new TransferCompletedConsumer(
                _processedMessages.Object,
                _outboxRepo.Object,
                _dbContext,
                NullLogger<TransferCompletedConsumer>.Instance);
        }

        [TearDown]
        public void TearDown() => _dbContext.Dispose();

        [Test]
        public async Task Consume_NewMessage_ShouldAddProcessedMessageAndTwoOutboxEntries()
        {
            var expectedKey = "TransferCompletedEvent:1";
            _processedMessages.Setup(x => x.ExistsAsync(expectedKey)).ReturnsAsync(false);

            var context = BuildContext(transferId: 1);
            await _consumer.Consume(context);

            _processedMessages.Verify(x => x.AddAsync(It.Is<ProcessedMessage>(m => m.Key == expectedKey)), Times.Once);
            _outboxRepo.Verify(x => x.AddAsync(It.Is<NotificationOutbox>(e => e.Email == "sender@test.com")), Times.Once);
            _outboxRepo.Verify(x => x.AddAsync(It.Is<NotificationOutbox>(e => e.Email == "receiver@test.com")), Times.Once);
        }

        [Test]
        public async Task Consume_DuplicateMessage_ShouldSkipProcessing()
        {
            var expectedKey = "TransferCompletedEvent:1";
            _processedMessages.Setup(x => x.ExistsAsync(expectedKey)).ReturnsAsync(true);

            var context = BuildContext(transferId: 1);
            await _consumer.Consume(context);

            _processedMessages.Verify(x => x.AddAsync(It.IsAny<ProcessedMessage>()), Times.Never);
            _outboxRepo.Verify(x => x.AddAsync(It.IsAny<NotificationOutbox>()), Times.Never);
        }

        private static ConsumeContext<TransferCompletedEvent> BuildContext(int transferId)
        {
            var evt = new TransferCompletedEvent(transferId, 10, "sender@test.com", 20, "receiver@test.com", 0.5m, "btc");
            var mock = new Mock<ConsumeContext<TransferCompletedEvent>>();
            mock.Setup(x => x.Message).Returns(evt);
            mock.Setup(x => x.CancellationToken).Returns(CancellationToken.None);
            return mock.Object;
        }
    }
}
