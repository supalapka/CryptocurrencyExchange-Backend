namespace CryptocurrencyExchange.Core.Domain.Wallet.Commands
{
    public class WalletTradeCommand
    {
        public string CoinSymbol { get; }
        public decimal CoinAmount { get; }
        public decimal CoinPrice { get; }

        public WalletTradeCommand(string coinSymbol, decimal coinAmount, decimal coinPrice)
        {
            CoinSymbol = coinSymbol.ToLower();
            CoinAmount = coinAmount;
            CoinPrice = coinPrice;
        }
    }
}
