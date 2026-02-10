using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Services.Futures;

namespace CryptocurrencyExchange.Core.Interfaces
{
    public interface IFuturesDomainService
    {
        Future OpenPosition(FutureDto futureDto, WalletItem usdtItem);
        void ClosePosition(Future position, WalletItem usdtItem, decimal pnl);
        void LiquidatePosition(Future position);
    }
}
