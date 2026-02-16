namespace CryptocurrencyExchange.Services.WalletTrade
{
    public class CoinTradeDto
    {
        public int UserId { get; set; }
        public string CoinSymbol { get; set; }
        public decimal CoinAmount { get; set; }
    }
}
