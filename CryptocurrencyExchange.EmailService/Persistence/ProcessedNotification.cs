namespace CryptocurrencyExchange.EmailService.Persistence
{
    public class ProcessedNotification
    {
        public int Id { get; private set; }
        public int OutboxEntryId { get; private set; }
        public DateTime ProcessedAt { get; private set; }

        private ProcessedNotification() { }

        public ProcessedNotification(int outboxEntryId)
        {
            OutboxEntryId = outboxEntryId;
            ProcessedAt = DateTime.UtcNow;
        }
    }
}
