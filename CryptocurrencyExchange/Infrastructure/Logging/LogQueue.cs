using CryptocurrencyExchange.Core.Models;
using System.Threading.Channels;

namespace CryptocurrencyExchange.Infrastructure.Logging
{
    public sealed class LogQueue
    {
        private readonly Channel<LogEntry> _channel;

        public LogQueue()
        {
            _channel = Channel.CreateBounded<LogEntry>(new BoundedChannelOptions(10_000)
            {
                FullMode = BoundedChannelFullMode.DropOldest,
                SingleReader = true,
                SingleWriter = false
            });
        }

        public ChannelWriter<LogEntry> Writer => _channel.Writer;
        public ChannelReader<LogEntry> Reader => _channel.Reader;
    }
}
