using CryptocurrencyExchange.Core.ValueObject;

namespace CryptocurrencyExchange.Application.Wallet
{
    public record CoinTradeDto(CoinSymbol CoinSymbol, decimal CoinAmount);
}
