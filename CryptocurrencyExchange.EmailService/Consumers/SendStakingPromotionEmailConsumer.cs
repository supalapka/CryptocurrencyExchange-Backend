using CryptocurrencyExchange.Core.Events;
using CryptocurrencyExchange.Core.ValueObject.User;
using CryptocurrencyExchange.EmailService.Interfaces;
using CryptocurrencyExchange.EmailService.Persistence;
using CryptocurrencyExchange.EmailService.Exceptions;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace CryptocurrencyExchange.EmailService.Consumers
{
    public class SendStakingPromotionEmailConsumer : IConsumer<SendStakingPromotionEmailCommand>
    {
        private const string EmailType = "StakingPromotion";
        private const int CooldownDays = 30;
        private const int DailyLimit = 200; // HARDODED VALUE FOR SMPT MAILING

        private readonly IEmailSender _emailSender;
        private readonly EmailDbContext _dbContext;
        private readonly ILogger<SendStakingPromotionEmailConsumer> _logger;

        public SendStakingPromotionEmailConsumer(
            IEmailSender emailSender,
            EmailDbContext dbContext,
            ILogger<SendStakingPromotionEmailConsumer> logger)
        {
            _emailSender = emailSender;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<SendStakingPromotionEmailCommand> context)
        {
            var message = context.Message;
            var email = new Email(message.Email);

            var todayStart = DateTime.UtcNow.Date;
            var sentToday = await _dbContext.SentEmails
                .CountAsync(e => e.SentAtUtc >= todayStart);

            if (sentToday >= DailyLimit)
                throw new EmailSendLimitExceededException(DailyLimit);

            var recentlySent = await _dbContext.SentEmails.AnyAsync(e =>
                e.EmailAddress == email.Value
                && e.EmailType == EmailType
                && e.SentAtUtc > DateTime.UtcNow.AddDays(-CooldownDays));

            if (recentlySent)
            {
                _logger.LogInformation(
                    "Skipping staking promotion for {Email}, sent within last {Days} days",
                    email.Value, CooldownDays);
                return;
            }

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

            _dbContext.SentEmails.Add(new SentEmail
            {
                EmailAddress = email.Value,
                EmailType = EmailType,
                SentAtUtc = DateTime.UtcNow
            });
            await _dbContext.SaveChangesAsync();
        }
    }
}
