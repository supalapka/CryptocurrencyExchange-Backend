namespace CryptocurrencyExchange.NotificationService.Entities
{
    public class ProcessedMessage
    {
        public int Id { get; private set; }
        public string Key { get; private set; }
        public DateTime ProcessedAt { get; private set; }

        private ProcessedMessage() { }

        public ProcessedMessage(string key)
        {
            Key = key;
            ProcessedAt = DateTime.UtcNow;
        }
    }
}
