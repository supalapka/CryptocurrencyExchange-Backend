using CryptocurrencyExchange.Models;
using CryptocurrencyExchange.Services.Interfaces;
using CryptocurrencyExchange.Utilities;

namespace CryptocurrencyExchange.Services.Wallet
{
    public class WalletDomainService : IWalletDomainService
    {
        public void Buy(WalletItem usdt, WalletItem coin, decimal usd, double coinPrice)
        {
            if ((decimal)usdt.Amount < usd)
                throw new InsufficientBalanceException("USDT");

            var amountToBuy = usd / (decimal)coinPrice;
            amountToBuy = (decimal)UtilFunсtions.RoundCoinAmountUpTo1USD((double)amountToBuy, coinPrice);

            usdt.Amount -= (double)usd;
            usdt.Amount = Math.Round(usdt.Amount, 2);

            coin.Amount += (double)amountToBuy;
            coin.Amount = UtilFunсtions.RoundCoinAmountUpTo1USD(coin.Amount, coinPrice);
        }
    }
}
