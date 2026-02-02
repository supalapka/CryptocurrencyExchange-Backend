using CryptocurrencyExchange.Exceptions;
using CryptocurrencyExchange.Models;
using CryptocurrencyExchange.Services.Interfaces;
using CryptocurrencyExchange.Utilities;

namespace CryptocurrencyExchange.Services.Wallet
{
    public class WalletDomainService : IWalletDomainService
    {
        public void Buy(WalletItem usdt, WalletItem coin, decimal usd, decimal coinPrice)
        {
            if (usdt.Amount < usd)
                throw new InsufficientFundsException("USDT");

            var amountToBuy = usd / coinPrice;
            amountToBuy = MoneyPolicyUtils.RoundCoinAmountUpTo1USD(amountToBuy, coinPrice);

            usdt.Amount -= usd;
            usdt.Amount = Math.Round(usdt.Amount, 2);

            coin.Amount += amountToBuy;
            coin.Amount = MoneyPolicyUtils.RoundCoinAmountUpTo1USD(coin.Amount, coinPrice);
        }

        public void Sell(WalletItem usdt, WalletItem coinToSell, decimal amount, decimal coinPrice)
        {
            var usdtAmount = coinPrice * amount;

            coinToSell.Amount -= amount;
            coinToSell.Amount = MoneyPolicyUtils.RoundCoinAmountUpTo1USD(coinToSell.Amount, coinPrice);

            usdt.Amount += usdtAmount;
            usdt.Amount = Math.Round(usdt.Amount, 2);
        }
    }
}
