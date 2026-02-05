using CryptocurrencyExchange.Models;

namespace CryptocurrencyExchange.Services.Interfaces
{
    public interface IFuturesDomainService
    {
        Future OpenPosition(FutureDto futureDto, WalletItem usdtItem);
        void ClosePosition(Future position, WalletItem usdtItem, decimal pnl);
        void LiquidatePosition(Future position);
    }
}
