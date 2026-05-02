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
    public class TransferOutboxDispatcherTests
    {
        private Mock<ITransferVerificationOutboxRepository> _outboxRepo;
        private Mock<IPublishEndpoint> _publishEndpoint;
        private Mock<IUnitOfWork> _uow;
        private TransferOutboxDispatcher _dispatcher;

        [SetUp]
        public void SetUp()
        {
            _outboxRepo = new Mock<ITransferVerificationOutboxRepository>();
            _publishEndpoint = new Mock<IPublishEndpoint>();
            _uow = new Mock<IUnitOfWork>();

            var services = new ServiceCollection();
            services.AddSingleton(_outboxRepo.Object);
            services.AddSingleton(_publishEndpoint.Object);
            services.AddSingleton(_uow.Object);

            var scopeFactory = services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>();
            _dispatcher = new TransferOutboxDispatcher(scopeFactory, NullLogger<TransferOutboxDispatcher>.Instance);
        }

        [Test]
        public async Task DispatchPendingAsync_WhenNoPendingEntries_DoesNotPublishOrCommit()
        {
            _outboxRepo.Setup(x => x.GetPendingAsync()).ReturnsAsync(new List<TransferVerificationOutbox>());

            await _dispatcher.DispatchPendingAsync();

            _publishEndpoint.Verify(
                x => x.Publish(It.IsAny<SendVerificationEmailCommand>(), It.IsAny<CancellationToken>()),
                Times.Never);
            _uow.Verify(x => x.CommitAsync(), Times.Never);
        }

        [Test]
        public async Task DispatchPendingAsync_WhenPendingEntriesExist_PublishesAndCommits()
        {
            var entry = new TransferVerificationOutbox("a@b.com", "123456");
            _outboxRepo.Setup(x => x.GetPendingAsync()).ReturnsAsync(new List<TransferVerificationOutbox> { entry });

            await _dispatcher.DispatchPendingAsync();

            _publishEndpoint.Verify(
                x => x.Publish(
                    It.Is<SendVerificationEmailCommand>(c => c.Email == "a@b.com" && c.VerificationCode == "123456"),
                    It.IsAny<CancellationToken>()),
                Times.Once);
            _uow.Verify(x => x.CommitAsync(), Times.Once);
        }

        [Test]
        public async Task DispatchPendingAsync_WhenPendingEntriesExist_MarksEntriesAsProcessed()
        {
            var entry = new TransferVerificationOutbox("a@b.com", "123456");
            _outboxRepo.Setup(x => x.GetPendingAsync()).ReturnsAsync(new List<TransferVerificationOutbox> { entry });

            await _dispatcher.DispatchPendingAsync();

            Assert.That(entry.ProcessedAt, Is.Not.Null);
        }
    }
}
