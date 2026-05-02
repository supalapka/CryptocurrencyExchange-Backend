using CryptocurrencyExchange.Core.Events;
using CryptocurrencyExchange.Core.Interfaces;
using CryptocurrencyExchange.Core.Interfaces.Repositories;
using MassTransit;

namespace CryptocurrencyExchange.Infrastructure.Outbox
{
    public class OutboxDispatcher : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OutboxDispatcher> _logger;

        public OutboxDispatcher(IServiceScopeFactory scopeFactory, ILogger<OutboxDispatcher> logger)
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
            var outboxRepository = scope.ServiceProvider.GetRequiredService<IUserRegistrationOutboxRepository>();
            var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var pending = await outboxRepository.GetPendingAsync();
            if (pending.Count == 0) return;

            foreach (var entry in pending)
            {
                await publishEndpoint.Publish(new UserRegisteredEvent(entry.UserId, entry.Email));
                entry.MarkProcessed();
            }

            await unitOfWork.CommitAsync();
            _logger.LogInformation("Dispatched {Count} registration outbox entries", pending.Count);
        }
    }
}
