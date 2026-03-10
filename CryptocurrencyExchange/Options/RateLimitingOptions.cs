namespace CryptocurrencyExchange.Options
{
    public class RateLimitingOptions
    {
        public int PermitLimit { get; init; }
        public int WindowSeconds { get; init; }
        public int QueueLimit { get; init; }
    }
}
