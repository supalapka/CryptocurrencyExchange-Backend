namespace CryptocurrencyExchange.EmailService.Options
{
    public class SmtpOptions
    {
        public string Host { get; init; } = null!;
        public int Port { get; init; } = 587;
        public string Username { get; init; } = null!;
        public string Password { get; set; } = null!;
        public string FromAddress { get; init; } = null!;
        public string FromName { get; init; } = "CryptocurrencyExchange";
        public bool UseSsl { get; init; } = true;
    }
}
