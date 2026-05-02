namespace CryptocurrencyExchange.Core.Models
{
    public class TransferVerificationOutbox
    {
        public int Id { get; private set; }
        public string Email { get; private set; }
        public string VerificationCode { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? ProcessedAt { get; private set; }

        private TransferVerificationOutbox() { }

        public TransferVerificationOutbox(string email, string verificationCode)
        {
            Email = email;
            VerificationCode = verificationCode;
            CreatedAt = DateTime.UtcNow;
        }

        public void MarkProcessed() => ProcessedAt = DateTime.UtcNow;
    }
}
