using CryptocurrencyExchange.Core.ValueObject;

namespace CryptocurrencyExchange.Services.WalletTrade
{
    public class CoinTradeDto
    {
        public CoinSymbol CoinSymbol { get; set; }
        public decimal CoinAmount { get; set; }
    }
}
