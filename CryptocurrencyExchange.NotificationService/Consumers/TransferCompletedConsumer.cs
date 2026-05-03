using CryptocurrencyExchange.Core.Events;
using CryptocurrencyExchange.NotificationService.Entities;
using CryptocurrencyExchange.NotificationService.Interfaces;
using CryptocurrencyExchange.NotificationService.Persistence;
using MassTransit;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CryptocurrencyExchange.NotificationService.Consumers
{
    public class TransferCompletedConsumer : IConsumer<TransferCompletedEvent>
    {
        private readonly IProcessedMessageRepository _processedMessages;
        private readonly INotificationOutboxRepository _outboxRepo;
        private readonly NotificationDbContext _dbContext;
        private readonly ILogger<TransferCompletedConsumer> _logger;

        public TransferCompletedConsumer(
            IProcessedMessageRepository processedMessages,
            INotificationOutboxRepository outbox,
            NotificationDbContext dbContext,
            ILogger<TransferCompletedConsumer> logger)
        {
            _processedMessages = processedMessages;
            _outboxRepo = outbox;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<TransferCompletedEvent> context)
        {
            TransferCompletedEvent transferCompleted = context.Message;
            var key = $"{nameof(TransferCompletedEvent)}:{transferCompleted.TransferId}";

            if (await _processedMessages.ExistsAsync(key))
            {
                _logger.LogInformation("Skipping duplicate event for transfer {TransferId}", transferCompleted.TransferId);
                return;
            }

            try
            {
                await _processedMessages.AddAsync(new ProcessedMessage(key));

                var senderNotification = new NotificationOutbox(
                    transferCompleted.SenderEmail,
                    $"Transfer #{transferCompleted.TransferId} sent",
                    $"Your transfer of {transferCompleted.Amount} {transferCompleted.Symbol} has been sent successfully.");

                var receiverNotification = new NotificationOutbox(
                    transferCompleted.ReceiverEmail,
                    $"Transfer #{transferCompleted.TransferId} received",
                    $"You have received {transferCompleted.Amount} {transferCompleted.Symbol}.");

                await _outboxRepo.AddAsync(senderNotification);
                await _outboxRepo.AddAsync(receiverNotification);

                await _dbContext.SaveChangesAsync(context.CancellationToken);

                _logger.LogInformation("Queued notification for transfer {TransferId}", transferCompleted.TransferId);
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2627 or 2601 })
            {
                _logger.LogInformation("Duplicate event for transfer {TransferId} caught at insert, skipping", transferCompleted.TransferId);
            }
        }
    }
}
