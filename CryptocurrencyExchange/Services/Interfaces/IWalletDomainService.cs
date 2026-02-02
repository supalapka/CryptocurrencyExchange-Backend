using CryptocurrencyExchange.Models;

namespace CryptocurrencyExchange.Services.Interfaces
{
    public interface IWalletDomainService
    {
        void Buy(WalletItem usdt, WalletItem coin, decimal usd, double coinPrice);
    }
}
