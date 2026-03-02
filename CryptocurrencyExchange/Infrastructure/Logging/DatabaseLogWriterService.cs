using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CryptocurrencyExchange.Infrastructure.Logging
{
    public sealed class DatabaseLogWriterService : BackgroundService
    {
        private readonly LogQueue _queue;
        private readonly IServiceProvider _serviceProvider;
        private const int BatchSize = 100;
        private static readonly TimeSpan FlushInterval = TimeSpan.FromSeconds(2);

        public DatabaseLogWriterService(LogQueue queue, IServiceProvider serviceProvider)
        {
            _queue = queue;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var batch = new List<LogEntry>(BatchSize);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                    cts.CancelAfter(FlushInterval);

                    try
                    {
                        var first = await _queue.Reader.ReadAsync(cts.Token);
                        batch.Add(first);
                    }
                    catch (OperationCanceledException)
                    {
                        // Timeout or shutdown — flush what we have
                    }

                    while (batch.Count < BatchSize && _queue.Reader.TryRead(out var item))
                        batch.Add(item);

                    if (batch.Count > 0)
                    {
                        await WriteBatchAsync(batch, stoppingToken);
                        batch.Clear();
                    }
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    Console.Error.WriteLine($"[DatabaseLogWriterService] Error writing log batch: {ex}");
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }

            // Drain remaining entries on shutdown
            while (_queue.Reader.TryRead(out var remaining))
                batch.Add(remaining);

            if (batch.Count > 0)
                await WriteBatchAsync(batch, CancellationToken.None);
        }

        private async Task WriteBatchAsync(List<LogEntry> entries, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DataContext>();

            await db.LogEntries.AddRangeAsync(entries, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);
        }
    }
}
