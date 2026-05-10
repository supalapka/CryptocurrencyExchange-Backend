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
    public class TransferCompletedOutboxDispatcherTests
    {
        private Mock<ITransferCompletedOutboxRepository> _outboxRepo;
        private Mock<IPublishEndpoint> _publishEndpoint;
        private Mock<IUnitOfWork> _uow;
        private TransferCompletedOutboxDispatcher _dispatcher;

        [SetUp]
        public void SetUp()
        {
            _outboxRepo = new Mock<ITransferCompletedOutboxRepository>();
            _publishEndpoint = new Mock<IPublishEndpoint>();
            _uow = new Mock<IUnitOfWork>();

            var services = new ServiceCollection();
            services.AddSingleton(_outboxRepo.Object);
            services.AddSingleton(_publishEndpoint.Object);
            services.AddSingleton(_uow.Object);

            var scopeFactory = services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>();
            _dispatcher = new TransferCompletedOutboxDispatcher(scopeFactory, NullLogger<TransferCompletedOutboxDispatcher>.Instance);
        }

        [Test]
        public async Task DispatchPendingAsync_WhenNoPendingEntries_DoesNotPublishOrCommit()
        {
            _outboxRepo.Setup(x => x.GetPendingAsync()).ReturnsAsync(new List<TransferCompletedOutbox>());

            await _dispatcher.DispatchPendingAsync();

            _publishEndpoint.Verify(
                x => x.Publish(It.IsAny<TransferCompletedEvent>(), It.IsAny<CancellationToken>()),
                Times.Never);
            _uow.Verify(x => x.CommitAsync(), Times.Never);
        }

        [Test]
        public async Task DispatchPendingAsync_WhenPendingEntriesExist_PublishesAndCommits()
        {
            var entry = new TransferCompletedOutbox(
                transferId: 7, senderId: 1, senderEmail: "s@b.com",
                receiverId: 2, receiverEmail: "r@b.com", amount: 5m, symbol: "btc");
            _outboxRepo.Setup(x => x.GetPendingAsync()).ReturnsAsync(new List<TransferCompletedOutbox> { entry });

            await _dispatcher.DispatchPendingAsync();

            _publishEndpoint.Verify(
                x => x.Publish(
                    It.Is<TransferCompletedEvent>(e =>
                        e.TransferId == 7 &&
                        e.SenderId == 1 &&
                        e.SenderEmail == "s@b.com" &&
                        e.ReceiverId == 2 &&
                        e.ReceiverEmail == "r@b.com" &&
                        e.Amount == 5m &&
                        e.Symbol == "btc"),
                    It.IsAny<CancellationToken>()),
                Times.Once);
            _uow.Verify(x => x.CommitAsync(), Times.Once);
        }

        [Test]
        public async Task DispatchPendingAsync_WhenPendingEntriesExist_MarksEntriesAsProcessed()
        {
            var entry = new TransferCompletedOutbox(
                transferId: 7, senderId: 1, senderEmail: "s@b.com",
                receiverId: 2, receiverEmail: "r@b.com", amount: 5m, symbol: "btc");
            _outboxRepo.Setup(x => x.GetPendingAsync()).ReturnsAsync(new List<TransferCompletedOutbox> { entry });

            await _dispatcher.DispatchPendingAsync();

            Assert.That(entry.ProcessedAt, Is.Not.Null);
        }
    }
}
