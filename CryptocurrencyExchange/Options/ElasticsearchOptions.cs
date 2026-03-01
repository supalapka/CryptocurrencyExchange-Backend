namespace CryptocurrencyExchange.Options
{
    public class ElasticsearchOptions
    {
        public string Uri         { get; init; } = null!;
        public string IndexFormat { get; init; } = "cryptocurrency-exchange-{0:yyyy.MM}";
        public string Username    { get; init; } = string.Empty;
        public string Password    { get; init; } = string.Empty;
    }
}
