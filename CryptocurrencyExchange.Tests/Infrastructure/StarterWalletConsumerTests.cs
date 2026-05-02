using CryptocurrencyExchange.Core.Events;
using CryptocurrencyExchange.Core.Interfaces;
using CryptocurrencyExchange.Core.Interfaces.Repositories;
using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Core.ValueObject;
using CryptocurrencyExchange.Infrastructure.Wallets;
using MassTransit;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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

        private static readonly CoinSymbol UsdtSymbol = new(CoinSymbol.Usdt.Value);

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
                c => c.Message == new UserRegisteredEvent(TestUser.DefaultId, "test@test.com"));

            await _consumer.Consume(context);

            _walletRepo.Verify(
                x => x.AddAsync(It.Is<WalletItem>(w =>
                    w.UserId == TestUser.DefaultId &&
                    w.Symbol == UsdtSymbol)),
                Times.Once);
            _uow.Verify(x => x.CommitAsync(), Times.Once);
        }

        [Test]
        public void Consume_WhenConcurrentInsertWins_DoesNotThrow()
        {
            _uow.Setup(x => x.CommitAsync())
                .ThrowsAsync(new DbUpdateException("duplicate", MakeDuplicateKeySqlException()));

            var context = Mock.Of<ConsumeContext<UserRegisteredEvent>>(
                c => c.Message == new UserRegisteredEvent(TestUser.DefaultId, "test@test.com"));

            Assert.DoesNotThrowAsync(async () => await _consumer.Consume(context));
        }

        [Test]
        public void Consume_WhenCommitFailsWithUnrelatedError_Rethrows()
        {
            _uow.Setup(x => x.CommitAsync())
                .ThrowsAsync(new DbUpdateException("other error"));

            var context = Mock.Of<ConsumeContext<UserRegisteredEvent>>(
                c => c.Message == new UserRegisteredEvent(TestUser.DefaultId, "test@test.com"));

            Assert.ThrowsAsync<DbUpdateException>(async () => await _consumer.Consume(context));
        }

        private static SqlException MakeDuplicateKeySqlException()
        {
            var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;

            var collection = (SqlErrorCollection)Activator.CreateInstance(typeof(SqlErrorCollection), nonPublic: true)!;

            var errorCtor = typeof(SqlError).GetConstructors(flags)
                .First(c => c.GetParameters().Length == 8);
            var error = (SqlError)errorCtor.Invoke(new object?[] { 2627, (byte)0, (byte)0, "server", "duplicate key", "proc", 0, null });

            typeof(SqlErrorCollection).GetMethod("Add", flags)!.Invoke(collection, new object[] { error });

            return (SqlException)typeof(SqlException).GetConstructors(flags)[0]
                .Invoke(new object?[] { "duplicate key", collection, null, Guid.NewGuid() });
        }
    }
}
