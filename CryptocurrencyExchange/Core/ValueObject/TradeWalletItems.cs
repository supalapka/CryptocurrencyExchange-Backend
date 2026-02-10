using CryptocurrencyExchange.Core.Models;

namespace CryptocurrencyExchange.Core.ValueObject
{
    public record TradeWalletItems(
     WalletItem BaseCurrency,
     WalletItem TradedCurrency
 );

}
