namespace CryptocurrencyExchange.Models
{
    public class Future
    {
        public int Id { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public double Margin { get; set; }
        public double EntryPrice { get; set; }
        public int UserId { get; set; }
        public bool IsCompleted { get; set; }
        public int Leverage { get; set; }
        public PositionType Position { get; set; }
    }
}
