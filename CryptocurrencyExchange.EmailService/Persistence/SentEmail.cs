namespace CryptocurrencyExchange.EmailService.Persistence
{
    public class SentEmail
    {
        public int Id { get; set; }
        public string EmailAddress { get; set; }
        public string EmailType { get; set; }
        public DateTime SentAtUtc { get; set; }
    }
}
