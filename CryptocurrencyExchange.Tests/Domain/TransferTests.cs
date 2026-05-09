using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Core.ValueObject;
using CryptocurrencyExchange.Exceptions;
using NUnit.Framework;

namespace CryptocurrencyExchange.Tests.Domain
{
    [TestFixture]
    internal class TransferTests
    {
        private const int SenderId = 1;
        private const int ReceiverId = 2;
        private const string Code = "123456";

        [Test]
        public void Create_SenderEqualsReceiver_ShouldThrowSelfTransferException()
        {
            Assert.Throws<SelfTransferException>(() =>
                Transfer.Create(SenderId, SenderId, CoinSymbol.Btc, 1m, Code));
        }

        [Test]
        public void Execute_CorrectCode_ShouldDeductSenderAndCreditReceiver()
        {
            var transfer = Transfer.Create(SenderId, ReceiverId, CoinSymbol.Btc, 1m, Code);
            var senderItem = WalletItemMother.CreateItem(SenderId, "btc", 5m);
            var receiverItem = WalletItemMother.CreateItem(ReceiverId, "btc", 0m);

            transfer.Execute(senderItem, receiverItem, new VerificationCode(Code));

            Assert.That(senderItem.Amount.Value, Is.EqualTo(4m));
            Assert.That(receiverItem.Amount.Value, Is.EqualTo(1m));
            Assert.That(transfer.Status, Is.EqualTo(TransferStatus.Completed));
        }

        [Test]
        public void Execute_WrongCode_ShouldThrowInvalidVerificationCodeException()
        {
            var transfer = Transfer.Create(SenderId, ReceiverId, CoinSymbol.Btc, 1m, Code);
            var senderItem = WalletItemMother.CreateItem(SenderId, "btc", 5m);
            var receiverItem = WalletItemMother.CreateItem(ReceiverId, "btc", 0m);

            Assert.Throws<InvalidVerificationCodeException>(() =>
                transfer.Execute(senderItem, receiverItem, new VerificationCode("999999")));
        }

        [Test]
        public void Execute_AlreadyCompleted_ShouldThrowInvalidOperationException()
        {
            var transfer = Transfer.Create(SenderId, ReceiverId, CoinSymbol.Btc, 1m, Code);
            var senderItem = WalletItemMother.CreateItem(SenderId, "btc", 5m);
            var receiverItem = WalletItemMother.CreateItem(ReceiverId, "btc", 0m);

            transfer.Execute(senderItem, receiverItem, new VerificationCode(Code));

            Assert.Throws<InvalidOperationException>(() =>
                transfer.Execute(senderItem, receiverItem, new VerificationCode(Code)));
        }

        [Test]
        public void Execute_InsufficientFunds_ShouldThrowInsufficientFundsException()
        {
            var transfer = Transfer.Create(SenderId, ReceiverId, CoinSymbol.Btc, 10m, Code);
            var senderItem = WalletItemMother.CreateItem(SenderId, "btc", 5m);
            var receiverItem = WalletItemMother.CreateItem(ReceiverId, "btc", 0m);

            Assert.Throws<InsufficientFundsException>(() =>
                transfer.Execute(senderItem, receiverItem, new VerificationCode(Code)));
        }
    }
}
