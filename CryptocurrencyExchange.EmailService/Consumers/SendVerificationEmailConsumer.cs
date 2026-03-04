using CryptocurrencyExchange.Core.Events;
using CryptocurrencyExchange.Core.ValueObject.User;
using CryptocurrencyExchange.EmailService.Interfaces;
using MassTransit;

namespace CryptocurrencyExchange.EmailService.Consumers
{
    public class SendVerificationEmailConsumer : IConsumer<SendVerificationEmailCommand>
    {
        private readonly IEmailSender _emailSender;
        private readonly ILogger<SendVerificationEmailConsumer> _logger;

        public SendVerificationEmailConsumer(IEmailSender emailSender, ILogger<SendVerificationEmailConsumer> logger)
        {
            _emailSender = emailSender;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<SendVerificationEmailCommand> context)
        {
            var command = context.Message;
            var email = new Email(command.Email);

            _logger.LogInformation("Processing verification email for {Email}", email.Value);

            var subject = "Your Verification Code";
            var body = $"<p>Your verification code is: <strong>{command.VerificationCode}</strong></p>";

            await _emailSender.SendAsync(email.Value, subject, body, context.CancellationToken);
        }
    }
}
