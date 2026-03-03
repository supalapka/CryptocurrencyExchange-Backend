using CryptocurrencyExchange.Application.Auth;
using CryptocurrencyExchange.Core.Interfaces;
using CryptocurrencyExchange.Core.Interfaces.Repositories;
using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Core.ValueObject.User;
using CryptocurrencyExchange.Exceptions;
using CryptocurrencyExchange.Services.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

namespace CryptocurrencyExchange.Tests.ServicesTests
{
    [TestFixture]
    public class AuthServiceTests
    {
        private Mock<IUserRepository> _userRepo;
        private Mock<IWalletItemRepository> _walletRepo;
        private Mock<IAuthDomainService> _authDomainService;
        private Mock<IUnitOfWork> _uow;
        private Mock<ITokenService> _tokenService;

        private AuthService _service;

        private const string TestEmail = "test@example.com";
        private const string TestPassword = "password123";

        [SetUp]
        public void SetUp()
        {
            _userRepo = new Mock<IUserRepository>();
            _walletRepo = new Mock<IWalletItemRepository>();
            _authDomainService = new Mock<IAuthDomainService>();
            _uow = new Mock<IUnitOfWork>();
            _tokenService = new Mock<ITokenService>();

            _uow.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()))
                .Returns<Func<Task>>(f => f());

            _service = new AuthService(
                _userRepo.Object,
                _uow.Object,
                _walletRepo.Object,
                _authDomainService.Object,
                _tokenService.Object,
                NullLogger<AuthService>.Instance
            );
        }

        [Test]
        public void LoginAsync_WhenUserNotFound_ThrowsUserNotFoundException()
        {
            _userRepo.Setup(x => x.GetByEmailAsync(TestEmail)).ReturnsAsync((User)null);

            Assert.ThrowsAsync<UserNotFoundException>(async () =>
                await _service.LoginAsync(TestEmail, TestPassword));
        }

        [Test]
        public void LoginAsync_WhenPasswordWrong_ThrowsInvalidPasswordException()
        {
            var user = CreateTestUser();
            _userRepo.Setup(x => x.GetByEmailAsync(TestEmail)).ReturnsAsync(user);
            _authDomainService.Setup(x => x.VerifyPassword(TestPassword, user)).Returns(false);

            Assert.ThrowsAsync<InvalidPasswordException>(async () =>
                await _service.LoginAsync(TestEmail, TestPassword));
        }

        [Test]
        public async Task LoginAsync_WhenCredentialsValid_ReturnsToken()
        {
            var user = CreateTestUser();
            _userRepo.Setup(x => x.GetByEmailAsync(TestEmail)).ReturnsAsync(user);
            _authDomainService.Setup(x => x.VerifyPassword(TestPassword, user)).Returns(true);
            _tokenService.Setup(x => x.CreateToken(user)).Returns("jwt-token");

            var result = await _service.LoginAsync(TestEmail, TestPassword);

            Assert.That(result, Is.EqualTo("jwt-token"));
        }

        [Test]
        public void RegisterAsync_WhenUserExists_ThrowsUserAlreadyExistsException()
        {
            _userRepo.Setup(x => x.UserExists(TestEmail)).ReturnsAsync(true);

            Assert.ThrowsAsync<UserAlreadyExistsException>(async () =>
                await _service.RegisterAsync(TestEmail, TestPassword));
        }

        [Test]
        public async Task RegisterAsync_WhenNewUser_CreatesUserAndWallet()
        {
            var user = CreateTestUser();
            _userRepo.Setup(x => x.UserExists(TestEmail)).ReturnsAsync(false);
            _authDomainService.Setup(x => x.CreateUser(TestEmail, TestPassword)).Returns(user);

            await _service.RegisterAsync(TestEmail, TestPassword);

            _userRepo.Verify(x => x.AddUserAsync(user), Times.Once);
            _walletRepo.Verify(x => x.AddAsync(It.IsAny<WalletItem>()), Times.Once);
            _uow.Verify(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()), Times.Once);
        }

        [Test]
        public void GetEmailByIdAsync_WhenUserNotFound_ThrowsUserNotFoundException()
        {
            _userRepo.Setup(x => x.GetEmailByIdAsync(TestUser.DefaultId)).ReturnsAsync((string)null);

            Assert.ThrowsAsync<UserNotFoundException>(async () =>
                await _service.GetEmailByIdAsync(TestUser.DefaultId));
        }

        [Test]
        public async Task GetEmailByIdAsync_WhenUserExists_ReturnsEmail()
        {
            _userRepo.Setup(x => x.GetEmailByIdAsync(TestUser.DefaultId)).ReturnsAsync(TestEmail);

            var result = await _service.GetEmailByIdAsync(TestUser.DefaultId);

            Assert.That(result, Is.EqualTo(TestEmail));
        }

        private static User CreateTestUser()
        {
            return new User(
                new Email(TestEmail),
                new byte[] { 1, 2, 3 },
                new byte[] { 4, 5, 6 }
            );
        }
    }
}
