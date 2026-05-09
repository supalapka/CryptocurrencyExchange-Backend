using CryptocurrencyExchange.Core.ValueObject;
using CryptocurrencyExchange.Exceptions;

namespace CryptocurrencyExchange.Core.Models
{
    public class WalletItem
    {
        public int UserId { get; private set; }
        public User User { get; private set; }
        public CoinSymbol Symbol { get; private set; }
        public Balance Amount { get; private set; }

        private WalletItem() { } // for ORM

        public WalletItem(int userId, CoinSymbol symbol)
        {
            UserId = userId;
            Symbol = symbol;
            Amount = Balance.Zero;
        }

        public WalletItem(User user, CoinSymbol symbol)
        {
            User = user;
            Symbol = symbol;
            Amount = Balance.Zero;
        }

        public bool HasSufficientBalance(decimal amount) => Amount.Value >= amount;

        public void AddAmount(decimal amount)
        {
            Amount += amount;
        }
        public void RemoveAmount(decimal amount)
        {
            if (Amount.Value < amount)
                throw new InsufficientFundsException(Symbol.Value);
            Amount -= amount;
        }
    }
}
