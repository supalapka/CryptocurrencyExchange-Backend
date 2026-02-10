using CryptocurrencyExchange.Core.ValueObject;

namespace CryptocurrencyExchange.Services.Futures
{
    public class FutureHIstoryOutput
    {
        public string Symbol { get; set; } = string.Empty;
        public decimal Margin { get; set; }
        public double EntryPrice { get; set; }
        public double MarkPrice { get; set; }
        public int Leverage { get; set; }
        public PositionType Position { get; set; }
        public bool IsLiquidated { get; set; }
    }
}
