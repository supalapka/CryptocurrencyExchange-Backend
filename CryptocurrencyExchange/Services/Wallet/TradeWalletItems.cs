using CryptocurrencyExchange.Models;

namespace CryptocurrencyExchange.Services.Wallet
{
    public record TradeWalletItems(
     WalletItem BaseCurrency,
     WalletItem TradedCurrency
 );

}
