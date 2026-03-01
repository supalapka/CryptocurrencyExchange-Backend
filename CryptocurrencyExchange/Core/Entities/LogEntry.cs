namespace CryptocurrencyExchange.Core.Models
{
    public class LogEntry
    {
        public int Id { get; set; }
        public string Level { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Exception { get; set; }
        public DateTime TimestampUtc { get; set; }
    }
}
