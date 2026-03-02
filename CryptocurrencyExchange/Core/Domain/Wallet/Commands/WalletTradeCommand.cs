using CryptocurrencyExchange.Core.ValueObject;

namespace CryptocurrencyExchange.Core.Domain.Wallet.Commands
{
    public class WalletTradeCommand
    {
        public CoinSymbol CoinSymbol { get; }
        public decimal CoinAmount { get; }
        public decimal CoinPrice { get; }

        public WalletTradeCommand(CoinSymbol coinSymbol, decimal coinAmount, decimal coinPrice)
        {
            CoinSymbol = coinSymbol;
            CoinAmount = coinAmount;
            CoinPrice = coinPrice;
        }
    }
}
