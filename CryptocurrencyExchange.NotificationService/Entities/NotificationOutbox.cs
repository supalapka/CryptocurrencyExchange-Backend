namespace CryptocurrencyExchange.NotificationService.Entities
{
    public class NotificationOutbox
    {
        public int Id { get; private set; }
        public string Email { get; private set; }
        public string Subject { get; private set; }
        public string Body { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? ProcessedAt { get; private set; }

        private NotificationOutbox() { }

        public NotificationOutbox(string email, string subject, string body)
        {
            Email = email;
            Subject = subject;
            Body = body;
            CreatedAt = DateTime.UtcNow;
        }

        public void MarkProcessed() => ProcessedAt = DateTime.UtcNow;
    }
}
