namespace CryptocurrencyExchange.Options
{
    public class RabbitMqOptions
    {
        public string Host { get; init; } = null!;
        public string VirtualHost { get; init; } = "/";
        public string Username { get; init; } = null!;
        public string Password { get; init; } = null!;
    }
}
