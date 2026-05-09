using CryptocurrencyExchange.Core.ValueObject;
using NUnit.Framework;

namespace CryptocurrencyExchange.Tests.Domain
{
    [TestFixture]
    internal class VerificationCodeTests
    {
        [Test]
        public void Generate_ProducesSixDigitCode()
        {
            var code = VerificationCode.Generate();

            Assert.That(code.Value.Length, Is.EqualTo(6));
            Assert.That(code.Value.All(char.IsDigit), Is.True);
        }

        [Test]
        public void Generate_ProducesDifferentCodesAcrossCalls()
        {
            var codes = Enumerable.Range(0, 50).Select(_ => VerificationCode.Generate().Value).ToHashSet();

            Assert.That(codes.Count, Is.GreaterThan(1));
        }
    }
}
