namespace CryptocurrencyExchange.Options
{
    public sealed class MarketApiResilienceOptions
    {
        public int TimeoutMs { get; init; }
        public int RetryDelayMs { get; init; }
    }
}
