using CryptocurrencyExchange.Application.Users;
using CryptocurrencyExchange.Core.Interfaces.Repositories;
using CryptocurrencyExchange.Exceptions;
using Moq;
using NUnit.Framework;

namespace CryptocurrencyExchange.Tests.ServicesTests
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IUserRepository> _userRepo;
        private UserService _service;

        [SetUp]
        public void SetUp()
        {
            _userRepo = new Mock<IUserRepository>();
            _service = new UserService(_userRepo.Object);
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
            _userRepo.Setup(x => x.GetEmailByIdAsync(TestUser.DefaultId)).ReturnsAsync("test@example.com");

            var result = await _service.GetEmailByIdAsync(TestUser.DefaultId);

            Assert.That(result, Is.EqualTo("test@example.com"));
        }
    }
}
