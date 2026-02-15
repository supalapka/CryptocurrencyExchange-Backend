using CryptocurrencyExchange.Core.Models;

namespace CryptocurrencyExchange.Tests
{
    internal class WalletItemMother
    {
        public static WalletItem CreateUsdt(decimal amount)
        {
            var item = new WalletItem(TestUser.DefaultId, "usdt");
            item.AddAmount(amount);

            return item;
        }

        public static WalletItem CreateBtc(decimal amount)
        {
            var item = new WalletItem(TestUser.DefaultId, "btc");
            item.AddAmount(amount);

            return item;
        }

        public static WalletItem CreateItem(string symbol, decimal amount)
        {
            var item = new WalletItem(TestUser.DefaultId, symbol);
            item.AddAmount(amount);

            return item;
        }
    }
}
