using CryptocurrencyExchange.Core.Models;
using Microsoft.Extensions.Logging;

namespace CryptocurrencyExchange.Infrastructure.Logging
{
    public sealed class DatabaseLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly LogQueue _queue;

        public DatabaseLogger(string categoryName, LogQueue queue)
        {
            _categoryName = categoryName;
            _queue = queue;
        }

        public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            _queue.Writer.TryWrite(new LogEntry
            {
                Level = logLevel.ToString(),
                Category = _categoryName,
                Message = formatter(state, exception),
                Exception = exception?.ToString(),
                TimestampUtc = DateTime.UtcNow
            });
        }

        private sealed class NullScope : IDisposable
        {
            public static NullScope Instance { get; } = new();
            public void Dispose() { }
        }
    }
}
