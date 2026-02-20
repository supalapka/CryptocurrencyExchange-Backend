namespace CryptocurrencyExchange.Core.Entities
{
    public class CryptoNews
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public string Coin { get; init; } = null!;
        public string Title { get; init; } = null!;
        public string Url { get; init; } = null!;
    }
}
