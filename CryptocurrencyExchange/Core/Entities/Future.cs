using CryptocurrencyExchange.Core.ValueObject;

namespace CryptocurrencyExchange.Core.Models
{
    public class Future
    {
        public int Id { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public decimal Margin { get; set; }
        public double EntryPrice { get; set; }
        public int UserId { get; set; }
        public bool IsCompleted { get; set; }
        public int Leverage { get; set; }
        public PositionType Position { get; set; }
    }
}
