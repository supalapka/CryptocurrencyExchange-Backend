namespace CryptocurrencyExchange.Core.Models
{
    public class TransferIdempotentRequest
    {
        public int Id { get; private set; }
        public string Key { get; private set; }
        public int UserId { get; private set; }
        public int? TransferId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime ExpiresAt { get; private set; }

        public bool IsExpired => DateTime.UtcNow > ExpiresAt;

        private TransferIdempotentRequest() { }

        public TransferIdempotentRequest(string key, int userId)
        {
            Key = key;
            UserId = userId;
            CreatedAt = DateTime.UtcNow;
            ExpiresAt = DateTime.UtcNow.AddHours(24);
        }

        public void SetTransferId(int transferId) => TransferId = transferId;
    }
}
