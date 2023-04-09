namespace CryptocurrencyExchange.Models
{
    public class WalletItem
    {
        public int Id { get; set; }
        public User Users { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public double Amount { get; set; }
    }
}
