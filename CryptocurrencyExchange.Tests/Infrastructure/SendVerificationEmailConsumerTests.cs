using CryptocurrencyExchange.Core.Events;
using CryptocurrencyExchange.EmailService.Interfaces;
using CryptocurrencyExchange.EmailService.Consumers;
using CryptocurrencyExchange.Exceptions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace CryptocurrencyExchange.Tests.Infrastructure
{
    [TestFixture]
    public class SendVerificationEmailConsumerTests
    {
        private Mock<IEmailSender> _emailSender;
        private SendVerificationEmailConsumer _consumer;

        [SetUp]
        public void SetUp()
        {
            _emailSender = new Mock<IEmailSender>();
            _consumer = new SendVerificationEmailConsumer(
                _emailSender.Object,
                Mock.Of<ILogger<SendVerificationEmailConsumer>>());
        }

        [Test]
        public async Task Consume_ValidEmail_SendsVerificationEmail()
        {
            var command = new SendVerificationEmailCommand("test@example.com", "123456");
            var context = Mock.Of<ConsumeContext<SendVerificationEmailCommand>>(
                c => c.Message == command && c.CancellationToken == CancellationToken.None);

            await _consumer.Consume(context);

            _emailSender.Verify(
                x => x.SendAsync(
                    "test@example.com",
                    It.IsAny<string>(),
                    It.Is<string>(b => b.Contains("123456")),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public void Consume_InvalidEmail_ThrowsInvalidEmailException()
        {
            var command = new SendVerificationEmailCommand("not-an-email", "123456");
            var context = Mock.Of<ConsumeContext<SendVerificationEmailCommand>>(
                c => c.Message == command && c.CancellationToken == CancellationToken.None);

            Assert.ThrowsAsync<InvalidEmailException>(() => _consumer.Consume(context));
        }

        [Test]
        public void Consume_WhenSendFails_ThrowsEmailSendingFailedException()
        {
            var command = new SendVerificationEmailCommand("test@example.com", "123456");
            var context = Mock.Of<ConsumeContext<SendVerificationEmailCommand>>(
                c => c.Message == command && c.CancellationToken == CancellationToken.None);

            _emailSender
                .Setup(x => x.SendAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new EmailSendingFailedException("test@example.com"));

            Assert.ThrowsAsync<EmailSendingFailedException>(() => _consumer.Consume(context));
        }
    }
}
