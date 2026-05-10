namespace CryptocurrencyExchange.Core.Models
{
    public class TransferCompletedOutbox
    {
        public int Id { get; private set; }
        public int TransferId { get; private set; }
        public int SenderId { get; private set; }
        public string SenderEmail { get; private set; }
        public int ReceiverId { get; private set; }
        public string ReceiverEmail { get; private set; }
        public decimal Amount { get; private set; }
        public string Symbol { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? ProcessedAt { get; private set; }

        private TransferCompletedOutbox() { }

        public TransferCompletedOutbox(
            int transferId,
            int senderId,
            string senderEmail,
            int receiverId,
            string receiverEmail,
            decimal amount,
            string symbol)
        {
            TransferId = transferId;
            SenderId = senderId;
            SenderEmail = senderEmail;
            ReceiverId = receiverId;
            ReceiverEmail = receiverEmail;
            Amount = amount;
            Symbol = symbol;
            CreatedAt = DateTime.UtcNow;
        }

        public void MarkProcessed() => ProcessedAt = DateTime.UtcNow;
    }
}
