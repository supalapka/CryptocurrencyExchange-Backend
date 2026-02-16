using CryptocurrencyExchange.Core.Domain.Wallet;
using CryptocurrencyExchange.Core.Domain.Wallet.Commands;
using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Core.ValueObject;
using CryptocurrencyExchange.Exceptions;

namespace CryptocurrencyExchange.Core.Domain.Wallets
{
    public class Wallet
    {
        private readonly Dictionary<string, WalletItem> _items;

        public int UserId { get; }

        public Wallet(int userId, IEnumerable<WalletItem> items)
        {
            UserId = userId;
            _items = items.ToDictionary(x => x.Symbol.Value);
        }

        public void Buy(WalletTradeCommand walletTradeCommand)
        {
            var usdt = GetRequired(CoinSymbol.Usdt);
            var coin = GetOrCreate(walletTradeCommand.CoinSymbol);

            var roundedCoinAmount = MoneyPolicy
                .RoundDownWithMax1UsdLoss(walletTradeCommand.CoinAmount, walletTradeCommand.CoinPrice);
            var usdToSpend = MoneyPolicy.RoundFiat(roundedCoinAmount * walletTradeCommand.CoinPrice);

            if (usdt.Amount < usdToSpend)
                throw new InsufficientFundsException(CoinSymbol.Usdt.Value);

            usdt.RemoveAmount(usdToSpend);
            coin.AddAmount(roundedCoinAmount);
        }

        public void Sell(WalletTradeCommand walletTradeCommand)
        {
            var usdt = GetRequired(new CoinSymbol(CoinSymbol.Usdt.Value));
            var coin = GetRequired(walletTradeCommand.CoinSymbol);

            if (coin.Amount < walletTradeCommand.CoinAmount)
                throw new InsufficientFundsException(walletTradeCommand.CoinSymbol.Value);

            var usdtAmount = walletTradeCommand.CoinAmount * walletTradeCommand.CoinPrice;
            usdtAmount = MoneyPolicy.RoundFiat(usdtAmount);

            usdt.AddAmount(usdtAmount);
            coin.RemoveAmount(walletTradeCommand.CoinAmount);
        }

        public decimal GetBalance(string symbol) =>
            _items.TryGetValue(symbol.ToLower(), out var item) ? item.Amount : 0;

        private WalletItem GetRequired(CoinSymbol symbol) =>
            _items.TryGetValue(symbol.Value, out var item)
                ? item
                : throw new InvalidOperationException($"Wallet item not found: {symbol}");

        private WalletItem GetOrCreate(CoinSymbol symbol)
        {
            if (_items.TryGetValue(symbol.Value, out var item))
                return item;

            item = new WalletItem(UserId, symbol);

            _items[symbol.Value] = item;
            return item;
        }

        public IReadOnlyCollection<WalletItem> Items => _items.Values;
    }
}
