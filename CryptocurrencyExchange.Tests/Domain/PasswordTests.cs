using CryptocurrencyExchange.Core.ValueObject.User;
using CryptocurrencyExchange.Exceptions;
using NUnit.Framework;

namespace CryptocurrencyExchange.Tests.Domain
{
    [TestFixture]
    public class PasswordTests
    {
        [Test]
        public void Constructor_WithValidValue_StoresValue()
        {
            var password = new Password("qwe");

            Assert.That(password.Value, Is.EqualTo("qwe"));
        }

        [Test]
        public void Constructor_WithNull_ThrowsInvalidPasswordException()
        {
            Assert.Throws<InvalidPasswordException>(() => new Password(null));
        }

        [Test]
        public void Constructor_WithEmptyString_ThrowsInvalidPasswordException()
        {
            Assert.Throws<InvalidPasswordException>(() => new Password(""));
        }

        [Test]
        public void ImplicitOperator_ConvertsToString()
        {
            string result = new Password("secret");

            Assert.That(result, Is.EqualTo("secret"));
        }

        [Test]
        public void ExplicitOperator_ConvertsFromString()
        {
            var password = (Password)"secret";

            Assert.That(password.Value, Is.EqualTo("secret"));
        }

        [Test]
        public void ToString_ReturnsMaskedValue()
        {
            var password = new Password("secret");

            Assert.That(password.ToString(), Is.EqualTo("***"));
        }
    }
}
