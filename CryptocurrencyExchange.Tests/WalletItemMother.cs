using CryptocurrencyExchange.Models;

namespace CryptocurrencyExchange.Tests
{
    internal class WalletItemMother
    {
        public static WalletItem CreateUsdt(int userId, decimal amount)
        {
            return new WalletItem
            {
                UserId = userId,
                Symbol = "usdt",
                Amount = amount
            };
        }

        public static WalletItem CreateItem(int userId,string symbol, decimal amount)
        {
            return new WalletItem
            {
                UserId = userId,
                Symbol = symbol.ToLower(),
                Amount = amount
            };
        }
    }
}
