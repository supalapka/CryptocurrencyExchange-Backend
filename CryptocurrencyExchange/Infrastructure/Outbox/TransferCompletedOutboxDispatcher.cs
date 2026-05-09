using CryptocurrencyExchange.Core.Events;
using CryptocurrencyExchange.Core.Interfaces;
using CryptocurrencyExchange.Core.Interfaces.Repositories;
using MassTransit;

namespace CryptocurrencyExchange.Infrastructure.Outbox
{
    public class TransferCompletedOutboxDispatcher : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<TransferCompletedOutboxDispatcher> _logger;

        public TransferCompletedOutboxDispatcher(IServiceScopeFactory scopeFactory, ILogger<TransferCompletedOutboxDispatcher> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await DispatchPendingAsync();
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        internal async Task DispatchPendingAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var outboxRepository = scope.ServiceProvider.GetRequiredService<ITransferCompletedOutboxRepository>();
            var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var pending = await outboxRepository.GetPendingAsync();
            if (pending.Count == 0) return;

            foreach (var entry in pending)
            {
                await publishEndpoint.Publish(new TransferCompletedEvent(
                    entry.TransferId,
                    entry.SenderId,
                    entry.SenderEmail,
                    entry.ReceiverId,
                    entry.ReceiverEmail,
                    entry.Amount,
                    entry.Symbol));
                entry.MarkProcessed();
            }

            await unitOfWork.CommitAsync();
            _logger.LogInformation("Dispatched {Count} transfer completed outbox entries", pending.Count);
        }
    }
}
