using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Exceptions;
using CryptocurrencyExchange.Utilities;

namespace CryptocurrencyExchange.Core.Domain.Wallet
{
    public class Wallet
    {
        private readonly Dictionary<string, WalletItem> _items;

        public int UserId { get; }

        public Wallet(int userId, IEnumerable<WalletItem> items)
        {
            UserId = userId;
            _items = items.ToDictionary(x => x.Symbol.ToLower());
        }

        public void Buy(string coinSymbol, decimal usd, decimal coinPrice)
        {
            var usdt = GetRequired("usdt");
            var coin = GetOrCreate(coinSymbol);

            if (usdt.Amount < usd)
                throw new InsufficientFundsException("USDT");

            var coinAmount = usd / coinPrice;
            coinAmount = MoneyPolicyUtils.RoundCoinAmountUpTo1USD(coinAmount, coinPrice);

            usdt.Amount = MoneyPolicyUtils.RoundFiat(usdt.Amount - usd);
            coin.Amount = MoneyPolicyUtils.RoundCoinAmountUpTo1USD(
                coin.Amount + coinAmount,
                coinPrice
            );
        }

        public void Sell(string coinSymbol, decimal amount, decimal coinPrice)
        {
            var usdt = GetRequired("usdt");
            var coin = GetRequired(coinSymbol);

            if (coin.Amount < amount)
                throw new InsufficientFundsException(coinSymbol.ToUpper());

            var usdtAmount = coinPrice * amount;

            coin.Amount = MoneyPolicyUtils.RoundCoinAmountUpTo1USD(
                coin.Amount - amount,
                coinPrice
            );

            usdt.Amount = MoneyPolicyUtils.RoundFiat(usdt.Amount + usdtAmount);
        }

        public decimal GetBalance(string symbol) =>
            _items.TryGetValue(symbol.ToLower(), out var item) ? item.Amount : 0;

        private WalletItem GetRequired(string symbol) =>
            _items.TryGetValue(symbol.ToLower(), out var item)
                ? item
                : throw new InvalidOperationException($"Wallet item not found: {symbol}");

        private WalletItem GetOrCreate(string symbol)
        {
            symbol = symbol.ToLower();

            if (_items.TryGetValue(symbol, out var item))
                return item;

            item = new WalletItem
            {
                Symbol = symbol,
                UserId = UserId,
                Amount = 0
            };

            _items[symbol] = item;
            return item;
        }

        public IReadOnlyCollection<WalletItem> Items => _items.Values;
    }
}
