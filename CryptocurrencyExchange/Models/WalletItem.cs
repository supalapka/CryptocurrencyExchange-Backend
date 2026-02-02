namespace CryptocurrencyExchange.Models
{
    public class WalletItem
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
