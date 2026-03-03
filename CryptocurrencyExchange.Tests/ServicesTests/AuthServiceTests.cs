using CryptocurrencyExchange.Application.Auth;
using CryptocurrencyExchange.Core.Events;
using CryptocurrencyExchange.Core.Interfaces;
using CryptocurrencyExchange.Core.Interfaces.Repositories;
using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Core.ValueObject.User;
using CryptocurrencyExchange.Exceptions;
using MassTransit;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

namespace CryptocurrencyExchange.Tests.ServicesTests
{
    [TestFixture]
    public class AuthServiceTests
    {
        private Mock<IUserRepository> _userRepo;
        private Mock<IAuthDomainService> _authDomainService;
        private Mock<IUnitOfWork> _uow;
        private Mock<ITokenService> _tokenService;
        private Mock<IPublishEndpoint> _publishEndpoint;

        private AuthService _service;

        private static readonly Email TestEmailVo = new("test@example.com");
        private static readonly Password TestPasswordVo = new("password123");

        [SetUp]
        public void SetUp()
        {
            _userRepo = new Mock<IUserRepository>();
            _authDomainService = new Mock<IAuthDomainService>();
            _uow = new Mock<IUnitOfWork>();
            _tokenService = new Mock<ITokenService>();
            _publishEndpoint = new Mock<IPublishEndpoint>();

            _uow.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()))
                .Returns<Func<Task>>(f => f());

            _service = new AuthService(
                _userRepo.Object,
                _uow.Object,
                _authDomainService.Object,
                _tokenService.Object,
                _publishEndpoint.Object,
                NullLogger<AuthService>.Instance
            );
        }

        [Test]
        public void LoginAsync_WhenUserNotFound_ThrowsUserNotFoundException()
        {
            _userRepo.Setup(x => x.GetByEmailAsync(TestEmailVo)).ReturnsAsync((User)null);

            Assert.ThrowsAsync<UserNotFoundException>(async () =>
                await _service.LoginAsync(TestEmailVo, TestPasswordVo));
        }

        [Test]
        public void LoginAsync_WhenPasswordWrong_ThrowsInvalidPasswordException()
        {
            var user = CreateTestUser();
            _userRepo.Setup(x => x.GetByEmailAsync(TestEmailVo)).ReturnsAsync(user);
            _authDomainService.Setup(x => x.VerifyPassword(TestPasswordVo, user)).Returns(false);

            Assert.ThrowsAsync<InvalidPasswordException>(async () =>
                await _service.LoginAsync(TestEmailVo, TestPasswordVo));
        }

        [Test]
        public async Task LoginAsync_WhenCredentialsValid_ReturnsToken()
        {
            var user = CreateTestUser();
            _userRepo.Setup(x => x.GetByEmailAsync(TestEmailVo)).ReturnsAsync(user);
            _authDomainService.Setup(x => x.VerifyPassword(TestPasswordVo, user)).Returns(true);
            _tokenService.Setup(x => x.CreateToken(user)).Returns("jwt-token");

            var result = await _service.LoginAsync(TestEmailVo, TestPasswordVo);

            Assert.That(result, Is.EqualTo("jwt-token"));
        }

        [Test]
        public void RegisterAsync_WhenUserExists_ThrowsUserAlreadyExistsException()
        {
            _userRepo.Setup(x => x.UserExists(TestEmailVo)).ReturnsAsync(true);

            Assert.ThrowsAsync<UserAlreadyExistsException>(async () =>
                await _service.RegisterAsync(TestEmailVo, TestPasswordVo));
        }

        [Test]
        public async Task RegisterAsync_WhenNewUser_CreatesUserAndPublishesEvent()
        {
            var user = CreateTestUser();
            _userRepo.Setup(x => x.UserExists(TestEmailVo)).ReturnsAsync(false);
            _authDomainService.Setup(x => x.CreateUser(TestEmailVo, TestPasswordVo)).Returns(user);

            await _service.RegisterAsync(TestEmailVo, TestPasswordVo);

            _userRepo.Verify(x => x.AddUserAsync(user), Times.Once);
            _uow.Verify(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()), Times.Once);
            _publishEndpoint.Verify(
                x => x.Publish(It.IsAny<UserRegisteredEvent>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        private static User CreateTestUser()
        {
            return new User(
                TestEmailVo,
                new byte[] { 1, 2, 3 },
                new byte[] { 4, 5, 6 }
            );
        }
    }
}
