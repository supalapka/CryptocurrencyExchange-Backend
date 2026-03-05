using CryptocurrencyExchange.Core.Events;
using CryptocurrencyExchange.Core.ValueObject.User;
using CryptocurrencyExchange.EmailService.Interfaces;
using MassTransit;

namespace CryptocurrencyExchange.EmailService.Consumers
{
    public class SendWelcomeEmailConsumer : IConsumer<UserRegisteredEvent>
    {
        private readonly IEmailSender _emailSender;
        private readonly ILogger<SendVerificationEmailConsumer> _logger;

        public SendWelcomeEmailConsumer(IEmailSender emailSender, ILogger<SendVerificationEmailConsumer> logger)
        {
            _emailSender = emailSender;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
        {
            var message = context.Message;
            var email = new Email(message.Email);

            _logger.LogInformation("Processing Welcome email for {Email}", email.Value);

            var subject = "Welcome to Cryptocurrency Exchange";
            var body = $@"
                <h2>Welcome to Cryptocurrency Exchange</h2>

                <p>Hello,</p>

                <p>Thank you for registering at <b>Cryptocurrency Exchange</b>. Your account has been successfully created.</p>

                <p>You can now:</p>
                <ul>
                    <li>Buy and sell cryptocurrencies</li>
                    <li>Trade futures</li>
                    <li>Stake your assets</li>
                </ul>

                <p>
                    If you did not create this account, please contact our 
                    <a href=""mailto:yurabalint0103@gmail.com"">support team</a> immediately.
                </p>

                <p>Best regards,<br/> Cryptocurrency Exchange Team</p>
                ";

            await _emailSender.SendAsync(email.Value, subject, body, context.CancellationToken);
        }
    }
}
