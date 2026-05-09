using CryptocurrencyExchange.Application.Transfers;
using CryptocurrencyExchange.Core.Events;
using CryptocurrencyExchange.Core.Interfaces;
using CryptocurrencyExchange.Core.Interfaces.Repositories;
using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Core.ValueObject;
using CryptocurrencyExchange.Core.ValueObject.User;
using CryptocurrencyExchange.Exceptions;
using MassTransit;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

namespace CryptocurrencyExchange.Tests.ServicesTests
{
    [TestFixture]
    public class TransferServiceTests
    {
        private Mock<ITransferRepository> _transferRepo;
        private Mock<IWalletItemRepository> _walletRepo;
        private Mock<IUserRepository> _userRepo;
        private Mock<IUnitOfWork> _uow;
        private Mock<ITransferVerificationOutboxRepository> _outboxRepo;
        private Mock<ITransferIdempotentRequestRepository> _idempotentRepo;
        private Mock<IPublishEndpoint> _publishEndpoint;

        private TransferService _service;

        private const int SenderId = 1;
        private const int ReceiverId = 2;
        private const string IdempotencyKey = "test-key-123";
        private static readonly Email SenderEmail = new("sender@test.com");
        private static readonly Email ReceiverEmail = new("receiver@test.com");

        [SetUp]
        public void SetUp()
        {
            _transferRepo = new Mock<ITransferRepository>();
            _walletRepo = new Mock<IWalletItemRepository>();
            _userRepo = new Mock<IUserRepository>();
            _uow = new Mock<IUnitOfWork>();
            _outboxRepo = new Mock<ITransferVerificationOutboxRepository>();
            _idempotentRepo = new Mock<ITransferIdempotentRequestRepository>();
            _publishEndpoint = new Mock<IPublishEndpoint>();

            _uow.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()))
                .Returns<Func<Task>>(f => f());

            _service = new TransferService(
                _transferRepo.Object,
                _walletRepo.Object,
                _userRepo.Object,
                _uow.Object,
                _outboxRepo.Object,
                _idempotentRepo.Object,
                _publishEndpoint.Object,
                NullLogger<TransferService>.Instance
            );
        }

        [Test]
        public async Task InitiateAsync_NewKey_ShouldCreateTransferAndWriteOutboxEntry()
        {
            var receiver = new User(ReceiverEmail, new byte[] { 1 }, new byte[] { 2 });
            var senderItem = new WalletItem(SenderId, CoinSymbol.Btc);
            senderItem.AddAmount(10m);
            SetUserProperty(senderItem, new User(SenderEmail, new byte[] { 3 }, new byte[] { 4 }));

            _idempotentRepo.Setup(x => x.FindAsync(IdempotencyKey, SenderId)).ReturnsAsync((TransferIdempotentRequest)null);
            _userRepo.Setup(x => x.GetByEmailAsync(ReceiverEmail)).ReturnsAsync(receiver);
            _walletRepo.Setup(x => x.GetWithUserAsync(SenderId, CoinSymbol.Btc)).ReturnsAsync(senderItem);

            var dto = new InitiateTransferDto(ReceiverEmail, "btc", 5m);
            await _service.InitiateAsync(SenderId, dto, IdempotencyKey);

            _transferRepo.Verify(x => x.AddAsync(It.IsAny<Transfer>()), Times.Once);
            _outboxRepo.Verify(
                x => x.AddAsync(It.Is<TransferVerificationOutbox>(e => e.Email == SenderEmail.Value)),
                Times.Once);
            _idempotentRepo.Verify(x => x.AddAsync(It.IsAny<TransferIdempotentRequest>()), Times.Once);
        }

        [Test]
        public async Task InitiateAsync_ExistingValidKey_ShouldReturnCachedTransferIdWithoutCreatingTransfer()
        {
            var existingRequest = CreateIdempotentRequest(IdempotencyKey, SenderId, transferId: 42);
            _idempotentRepo.Setup(x => x.FindAsync(IdempotencyKey, SenderId)).ReturnsAsync(existingRequest);

            var dto = new InitiateTransferDto(ReceiverEmail, "btc", 5m);
            var result = await _service.InitiateAsync(SenderId, dto, IdempotencyKey);

            Assert.That(result, Is.EqualTo(42));
            _transferRepo.Verify(x => x.AddAsync(It.IsAny<Transfer>()), Times.Never);
            _outboxRepo.Verify(x => x.AddAsync(It.IsAny<TransferVerificationOutbox>()), Times.Never);
        }

        [Test]
        public async Task InitiateAsync_ConcurrentConflict_ShouldReturnExistingTransferId()
        {
            _idempotentRepo.Setup(x => x.FindAsync(IdempotencyKey, SenderId)).ReturnsAsync((TransferIdempotentRequest)null);

            var receiver = new User(ReceiverEmail, new byte[] { 1 }, new byte[] { 2 });
            var senderItem = new WalletItem(SenderId, CoinSymbol.Btc);
            senderItem.AddAmount(10m);
            SetUserProperty(senderItem, new User(SenderEmail, new byte[] { 3 }, new byte[] { 4 }));

            _userRepo.Setup(x => x.GetByEmailAsync(ReceiverEmail)).ReturnsAsync(receiver);
            _walletRepo.Setup(x => x.GetWithUserAsync(SenderId, CoinSymbol.Btc)).ReturnsAsync(senderItem);

            _uow.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()))
                .ThrowsAsync(new DbUpdateException("duplicate key", MakeDuplicateKeySqlException()));

            var conflictRecord = CreateIdempotentRequest(IdempotencyKey, SenderId, transferId: 99);
            _idempotentRepo.Setup(x => x.FindAsync(IdempotencyKey, SenderId)).ReturnsAsync(conflictRecord);

            var dto = new InitiateTransferDto(ReceiverEmail, "btc", 5m);
            var result = await _service.InitiateAsync(SenderId, dto, IdempotencyKey);

            Assert.That(result, Is.EqualTo(99));
        }

        [Test]
        public void InitiateAsync_ReceiverNotFound_ShouldThrowUserNotFoundException()
        {
            _idempotentRepo.Setup(x => x.FindAsync(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync((TransferIdempotentRequest)null);
            _userRepo.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);

            var dto = new InitiateTransferDto("unknown@test.com", "btc", 5m);

            Assert.ThrowsAsync<UserNotFoundException>(async () =>
                await _service.InitiateAsync(SenderId, dto, IdempotencyKey));
        }

        [Test]
        public void InitiateAsync_SelfTransfer_ShouldThrowSelfTransferException()
        {
            var sender = new User(SenderEmail, new byte[] { 1 }, new byte[] { 2 });
            var senderItem = new WalletItem(sender.Id, CoinSymbol.Btc);
            senderItem.AddAmount(10m);

            _idempotentRepo.Setup(x => x.FindAsync(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync((TransferIdempotentRequest)null);
            _userRepo.Setup(x => x.GetByEmailAsync(SenderEmail)).ReturnsAsync(sender);
            _walletRepo.Setup(x => x.GetWithUserAsync(sender.Id, It.IsAny<CoinSymbol>())).ReturnsAsync(senderItem);

            var dto = new InitiateTransferDto(SenderEmail, "btc", 5m);

            Assert.ThrowsAsync<SelfTransferException>(async () =>
                await _service.InitiateAsync(sender.Id, dto, IdempotencyKey));
        }

        [Test]
        public async Task ConfirmAsync_ValidCode_ShouldCompleteTransfer()
        {
            var transfer = Transfer.Create(SenderId, ReceiverId, CoinSymbol.Btc, 5m, new VerificationCode("123456"));
            var senderItem = WalletItemMother.CreateItem(SenderId, "btc", 10m);
            var receiverItem = WalletItemMother.CreateItem(ReceiverId, "btc", 0m);

            _transferRepo.Setup(x => x.GetPendingByIdAndSenderAsync(0, SenderId)).ReturnsAsync(transfer);
            _walletRepo.Setup(x => x.GetForTransferAsync(SenderId, ReceiverId, CoinSymbol.Btc))
                .ReturnsAsync((senderItem, receiverItem));

            var dto = new ConfirmTransferDto(0, "123456");
            await _service.ConfirmAsync(SenderId, dto);

            Assert.That(senderItem.Amount.Value, Is.EqualTo(5m));
            Assert.That(receiverItem.Amount.Value, Is.EqualTo(5m));
            Assert.That(transfer.Status, Is.EqualTo(TransferStatus.Completed));
        }

        [Test]
        public async Task ConfirmAsync_AlreadyCompleted_ShouldReturnWithoutError()
        {
            var completed = Transfer.Create(SenderId, ReceiverId, CoinSymbol.Btc, 5m, new VerificationCode("123456"));

            _transferRepo.Setup(x => x.GetPendingByIdAndSenderAsync(0, SenderId)).ReturnsAsync((Transfer)null);
            _transferRepo.Setup(x => x.GetCompletedByIdAndSenderAsync(0, SenderId)).ReturnsAsync(completed);

            var dto = new ConfirmTransferDto(0, "123456");

            Assert.DoesNotThrowAsync(async () => await _service.ConfirmAsync(SenderId, dto));
        }

        [Test]
        public void ConfirmAsync_TransferNotFound_ShouldThrowTransferNotFoundException()
        {
            _transferRepo.Setup(x => x.GetPendingByIdAndSenderAsync(99, SenderId)).ReturnsAsync((Transfer)null);
            _transferRepo.Setup(x => x.GetCompletedByIdAndSenderAsync(99, SenderId)).ReturnsAsync((Transfer)null);

            var dto = new ConfirmTransferDto(99, "123456");

            Assert.ThrowsAsync<TransferNotFoundException>(async () =>
                await _service.ConfirmAsync(SenderId, dto));
        }

        [Test]
        public void ConfirmAsync_WrongCode_ShouldThrowInvalidVerificationCodeException()
        {
            var transfer = Transfer.Create(SenderId, ReceiverId, CoinSymbol.Btc, 5m, new VerificationCode("123456"));
            var senderItem = WalletItemMother.CreateItem(SenderId, "btc", 10m);
            var receiverItem = WalletItemMother.CreateItem(ReceiverId, "btc", 0m);

            _transferRepo.Setup(x => x.GetPendingByIdAndSenderAsync(0, SenderId)).ReturnsAsync(transfer);
            _walletRepo.Setup(x => x.GetForTransferAsync(SenderId, ReceiverId, CoinSymbol.Btc))
                .ReturnsAsync((senderItem, receiverItem));

            var dto = new ConfirmTransferDto(0, "999999");

            Assert.ThrowsAsync<InvalidVerificationCodeException>(async () =>
                await _service.ConfirmAsync(SenderId, dto));
        }

        [Test]
        public async Task ConfirmAsync_ReceiverHasNoWalletItem_ShouldCreateAndTransfer()
        {
            var transfer = Transfer.Create(SenderId, ReceiverId, CoinSymbol.Btc, 5m, new VerificationCode("123456"));
            var senderItem = WalletItemMother.CreateItem(SenderId, "btc", 10m);

            _transferRepo.Setup(x => x.GetPendingByIdAndSenderAsync(0, SenderId)).ReturnsAsync(transfer);
            _walletRepo.Setup(x => x.GetForTransferAsync(SenderId, ReceiverId, CoinSymbol.Btc))
                .ReturnsAsync((senderItem, null));

            var dto = new ConfirmTransferDto(0, "123456");
            await _service.ConfirmAsync(SenderId, dto);

            _walletRepo.Verify(x => x.AddAsync(It.IsAny<WalletItem>()), Times.Once);
            Assert.That(senderItem.Amount.Value, Is.EqualTo(5m));
        }

        [Test]
        public async Task ConfirmAsync_ValidCode_ShouldPublishTransferCompletedEvent()
        {
            var transfer = Transfer.Create(SenderId, ReceiverId, CoinSymbol.Btc, 5m, new VerificationCode("123456"));
            var senderItem = WalletItemMother.CreateItem(SenderId, "btc", 10m);
            var receiverItem = WalletItemMother.CreateItem(ReceiverId, "btc", 0m);

            _transferRepo.Setup(x => x.GetPendingByIdAndSenderAsync(0, SenderId)).ReturnsAsync(transfer);
            _walletRepo.Setup(x => x.GetForTransferAsync(SenderId, ReceiverId, CoinSymbol.Btc))
                .ReturnsAsync((senderItem, receiverItem));
            _userRepo.Setup(x => x.GetEmailByIdAsync(SenderId)).ReturnsAsync(SenderEmail.Value);
            _userRepo.Setup(x => x.GetEmailByIdAsync(ReceiverId)).ReturnsAsync(ReceiverEmail.Value);

            var dto = new ConfirmTransferDto(0, "123456");
            await _service.ConfirmAsync(SenderId, dto);

            _publishEndpoint.Verify(
                x => x.Publish(
                    It.Is<TransferCompletedEvent>(e =>
                        e.SenderId == SenderId &&
                        e.SenderEmail == SenderEmail.Value &&
                        e.ReceiverId == ReceiverId &&
                        e.ReceiverEmail == ReceiverEmail.Value &&
                        e.Amount == 5m &&
                        e.Symbol == "btc"),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        private static TransferIdempotentRequest CreateIdempotentRequest(string key, int userId, int transferId)
        {
            var request = new TransferIdempotentRequest(key, userId);
            request.SetTransferId(transferId);
            return request;
        }

        private static void SetUserProperty(WalletItem item, User user)
        {
            var prop = typeof(WalletItem).GetProperty("User");
            prop!.SetValue(item, user);
        }

        private static SqlException MakeDuplicateKeySqlException()
        {
            var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
            var collection = (SqlErrorCollection)Activator.CreateInstance(typeof(SqlErrorCollection), nonPublic: true)!;
            var errorCtor = typeof(SqlError).GetConstructors(flags).First(c => c.GetParameters().Length == 8);
            var error = (SqlError)errorCtor.Invoke(new object?[] { 2627, (byte)0, (byte)0, "server", "duplicate key", "proc", 0, null });
            typeof(SqlErrorCollection).GetMethod("Add", flags)!.Invoke(collection, new object[] { error });
            return (SqlException)typeof(SqlException).GetConstructors(flags)[0]
                .Invoke(new object?[] { "duplicate key", collection, null, Guid.NewGuid() });
        }
    }
}
