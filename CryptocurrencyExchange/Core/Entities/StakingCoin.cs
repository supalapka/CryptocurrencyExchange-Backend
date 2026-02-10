namespace CryptocurrencyExchange.Core.Models
{
    public class StakingCoin
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public float RatePerMonth { get; set; } // example 1.2% per month
        public string Description { get; set; }
    }
}
