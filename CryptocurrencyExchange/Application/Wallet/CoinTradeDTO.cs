using CryptocurrencyExchange.Core.ValueObject;

namespace CryptocurrencyExchange.Application.Wallet
{
    public class CoinTradeDto
    {
        public CoinSymbol CoinSymbol { get; set; }
        public decimal CoinAmount { get; set; }
    }
}
