namespace CryptocurrencyExchange.Options
{
    public class JwtOptions
    {
        public string SecretKey { get; init; } = string.Empty;
        public int ExpirationDays { get; init; }
    }
}
