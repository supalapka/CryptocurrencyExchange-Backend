using CryptocurrencyExchange.Core.Events;
using CryptocurrencyExchange.Core.ValueObject.User;
using CryptocurrencyExchange.EmailService.Interfaces;
using MassTransit;

namespace CryptocurrencyExchange.EmailService.Consumers
{
    public class SendStakingPromotionEmailConsumer : IConsumer<SendStakingPromotionEmailCommand>
    {
        private readonly IEmailSender _emailSender;
        private readonly ILogger<SendStakingPromotionEmailConsumer> _logger;

        public SendStakingPromotionEmailConsumer(IEmailSender emailSender, ILogger<SendStakingPromotionEmailConsumer> logger)
        {
            _emailSender = emailSender;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<SendStakingPromotionEmailCommand> context)
        {
            var message = context.Message;
            var email = new Email(message.Email);

            _logger.LogInformation("Sending staking promotion email to {Email}", email.Value);

            var subject = "Start Earning with Staking";
            var body = $@"
                <h2>Your USDT Could Be Earning Rewards</h2>

                <p>Hello,</p>

                <p>We noticed you have USDT in your wallet but haven't started staking yet.</p>

                <p>With staking, you can:</p>
                <ul>
                    <li>Earn passive rewards on your holdings</li>
                    <li>Choose from multiple coins and durations</li>
                    <li>Watch your portfolio grow over time</li>
                </ul>

                <p>Log in now and start staking to make your assets work for you.</p>

                <p>Best regards,<br/> Cryptocurrency Exchange Team</p>
                ";

            await _emailSender.SendAsync(email.Value, subject, body, context.CancellationToken);
        }
    }
}
