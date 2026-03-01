using Microsoft.Extensions.Logging;

namespace CryptocurrencyExchange.Infrastructure.Logging
{
    [ProviderAlias("Database")]
    public sealed class DatabaseLoggerProvider : ILoggerProvider
    {
        private readonly LogQueue _queue;

        public DatabaseLoggerProvider(LogQueue queue)
        {
            _queue = queue;
        }

        public ILogger CreateLogger(string categoryName)
            => new DatabaseLogger(categoryName, _queue);

        public void Dispose() { }
    }
}
