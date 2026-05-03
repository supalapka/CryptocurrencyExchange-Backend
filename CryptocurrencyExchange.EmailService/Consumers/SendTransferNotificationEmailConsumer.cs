using CryptocurrencyExchange.Core.Events;
using CryptocurrencyExchange.EmailService.Interfaces;
using CryptocurrencyExchange.EmailService.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace CryptocurrencyExchange.EmailService.Consumers
{
    public class SendTransferNotificationEmailConsumer : IConsumer<SendTransferNotificationEmailCommand>
    {
        private readonly IEmailSender _emailSender;
        private readonly EmailDbContext _dbContext;
        private readonly ILogger<SendTransferNotificationEmailConsumer> _logger;

        public SendTransferNotificationEmailConsumer(
            IEmailSender emailSender,
            EmailDbContext dbContext,
            ILogger<SendTransferNotificationEmailConsumer> logger)
        {
            _emailSender = emailSender;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<SendTransferNotificationEmailCommand> context)
        {
            var command = context.Message;

            var alreadyProcessed = await _dbContext.ProcessedNotifications
                .AnyAsync(x => x.OutboxEntryId == command.OutboxEntryId, context.CancellationToken);

            if (alreadyProcessed)
            {
                _logger.LogInformation("Skipping duplicate notification for outbox entry {OutboxEntryId}", command.OutboxEntryId);
                return;
            }

            try
            {
                _dbContext.ProcessedNotifications.Add(new ProcessedNotification(command.OutboxEntryId));
                await _dbContext.SaveChangesAsync(context.CancellationToken);
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2627 or 2601 })
            {
                _logger.LogInformation("Duplicate outbox entry {OutboxEntryId} caught at insert, skipping", command.OutboxEntryId);
                return;
            }

            await _emailSender.SendAsync(command.Email, command.Subject, command.Body, context.CancellationToken);
            _logger.LogInformation("Sent transfer notification email to {Email}", command.Email);
        }
    }
}
