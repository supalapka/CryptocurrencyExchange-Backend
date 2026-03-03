using CryptocurrencyExchange.Core.Events;
using CryptocurrencyExchange.Core.Interfaces;
using CryptocurrencyExchange.Core.Interfaces.Repositories;
using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Core.ValueObject;
using CryptocurrencyExchange.Infrastructure.Wallets;
using MassTransit;
using Moq;
using NUnit.Framework;

namespace CryptocurrencyExchange.Tests.Infrastructure
{
    [TestFixture]
    public class StarterWalletConsumerTests
    {
        private Mock<IWalletItemRepository> _walletRepo;
        private Mock<IUnitOfWork> _uow;
        private StarterWalletConsumer _consumer;

        [SetUp]
        public void SetUp()
        {
            _walletRepo = new Mock<IWalletItemRepository>();
            _uow = new Mock<IUnitOfWork>();
            _consumer = new StarterWalletConsumer(_walletRepo.Object, _uow.Object);
        }

        [Test]
        public async Task Consume_CreatesWalletAndCommits()
        {
            var context = Mock.Of<ConsumeContext<UserRegisteredEvent>>(
                c => c.Message == new UserRegisteredEvent(TestUser.DefaultId));

            await _consumer.Consume(context);

            _walletRepo.Verify(
                x => x.AddAsync(It.Is<WalletItem>(w =>
                    w.UserId == TestUser.DefaultId &&
                    w.Symbol == new CoinSymbol(CoinSymbol.Usdt.Value))),
                Times.Once);
            _uow.Verify(x => x.CommitAsync(), Times.Once);
        }
    }
}
