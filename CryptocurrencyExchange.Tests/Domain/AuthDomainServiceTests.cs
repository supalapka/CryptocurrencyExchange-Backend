using CryptocurrencyExchange.Core.Domain;
using CryptocurrencyExchange.Core.ValueObject.User;
using NUnit.Framework;

namespace CryptocurrencyExchange.Tests.Domain
{
    [TestFixture]
    public class AuthDomainServiceTests
    {
        private AuthDomainService _service;

        [SetUp]
        public void SetUp()
        {
            _service = new AuthDomainService();
        }

        [Test]
        public void CreateUser_ReturnsUserWithHashedPassword()
        {
            var user = _service.CreateUser(new Email("test@example.com"), new Password("password123"));

            Assert.That(user, Is.Not.Null);
            Assert.That(user.Email.Value, Is.EqualTo("test@example.com"));
            Assert.That(user.PasswordHash, Is.Not.Empty);
            Assert.That(user.PasswordSalt, Is.Not.Empty);
        }

        [Test]
        public void VerifyPassword_WithCorrectPassword_ReturnsTrue()
        {
            var user = _service.CreateUser(new Email("test@example.com"), new Password("password123"));
            var result = _service.VerifyPassword(new Password("password123"), user);

            Assert.That(result, Is.True);
        }

        [Test]
        public void VerifyPassword_WithWrongPassword_ReturnsFalse()
        {
            var user = _service.CreateUser(new Email("test@example.com"), new Password("password123"));
            var result = _service.VerifyPassword(new Password("wrongpassword"), user);

            Assert.That(result, Is.False);
        }
    }
}
