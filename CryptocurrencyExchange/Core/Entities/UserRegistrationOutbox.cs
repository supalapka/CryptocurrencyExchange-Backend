namespace CryptocurrencyExchange.Core.Models
{
    public class UserRegistrationOutbox
    {
        public int Id { get; private set; }
        public int UserId { get; private set; }
        public string Email { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? ProcessedAt { get; private set; }

        private UserRegistrationOutbox() { }

        public UserRegistrationOutbox(int userId, string email)
        {
            UserId = userId;
            Email = email;
            CreatedAt = DateTime.UtcNow;
        }

        public void MarkProcessed() => ProcessedAt = DateTime.UtcNow;
    }
}
