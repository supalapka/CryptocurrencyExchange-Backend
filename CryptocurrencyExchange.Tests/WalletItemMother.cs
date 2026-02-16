using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Core.ValueObject;

namespace CryptocurrencyExchange.Tests
{
    internal class WalletItemMother
    {
        public static WalletItem CreateUsdt(decimal amount)
        {
            var item = new WalletItem(TestUser.DefaultId, new CoinSymbol("usdt"));
            item.AddAmount(amount);

            return item;
        }

        public static WalletItem CreateBtc(decimal amount)
        {
            var item = new WalletItem(TestUser.DefaultId, new CoinSymbol("btc"));
            item.AddAmount(amount);

            return item;
        }

        public static WalletItem CreateItem(string symbol, decimal amount)
        {
            var item = new WalletItem(TestUser.DefaultId, new CoinSymbol(symbol));
            item.AddAmount(amount);

            return item;
        }
    }
}
