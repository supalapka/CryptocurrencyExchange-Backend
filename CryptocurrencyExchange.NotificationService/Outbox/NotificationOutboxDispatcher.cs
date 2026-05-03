using CryptocurrencyExchange.Core.Events;
using CryptocurrencyExchange.NotificationService.Interfaces;
using MassTransit;

namespace CryptocurrencyExchange.NotificationService.Outbox
{
    public class NotificationOutboxDispatcher : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<NotificationOutboxDispatcher> _logger;

        public NotificationOutboxDispatcher(IServiceScopeFactory scopeFactory, ILogger<NotificationOutboxDispatcher> logger)
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
            var outboxRepo = scope.ServiceProvider.GetRequiredService<INotificationOutboxRepository>();
            var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
            var dbContext = scope.ServiceProvider.GetRequiredService<Persistence.NotificationDbContext>();

            var pending = await outboxRepo.GetPendingAsync();
            if (pending.Count == 0) return;

            foreach (var entry in pending)
            {
                await publishEndpoint.Publish(new SendTransferNotificationEmailCommand(entry.Id, entry.Email, entry.Subject, entry.Body));
                entry.MarkProcessed();
            }

            await dbContext.SaveChangesAsync();
            _logger.LogInformation("Dispatched {Count} notification outbox entries", pending.Count);
        }
    }
}
