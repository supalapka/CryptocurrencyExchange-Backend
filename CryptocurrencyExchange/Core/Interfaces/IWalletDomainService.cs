using CryptocurrencyExchange.Core.Models;

namespace CryptocurrencyExchange.Core.Interfaces
{
    public interface IWalletDomainService
    {
        void Buy(WalletItem usdt, WalletItem coin, decimal usd, decimal coinPrice);
        void Sell(WalletItem usdt, WalletItem coin, decimal amount, decimal coinPrice);
    }
}
