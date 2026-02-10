using CryptocurrencyExchange.Core.Models;

namespace CryptocurrencyExchange.Tests
{
    internal class WalletItemMother
    {
        public static WalletItem CreateUsdt(decimal amount)
        {
            return new WalletItem
            {
                UserId = TestUser.DefaultId,
                Symbol = "usdt",
                Amount = amount
            };
        }

        public static WalletItem CreateBtc(decimal amount)
        {
            return new WalletItem
            {
                UserId = TestUser.DefaultId,
                Symbol = "btc",
                Amount = amount
            };
        }

        public static WalletItem CreateItem(string symbol, decimal amount)
        {
            return new WalletItem
            {
                UserId = TestUser.DefaultId,
                Symbol = symbol.ToLower(),
                Amount = amount
            };
        }
    }
}
