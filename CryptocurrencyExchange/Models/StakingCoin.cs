namespace CryptocurrencyExchange.Models
{
    public class StakingCoin
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public float RatePerMonth { get; set; } // example 10% per year
    }
}
