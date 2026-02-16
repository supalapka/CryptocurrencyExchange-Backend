using CryptocurrencyExchange.Core.ValueObject;
using CryptocurrencyExchange.Exceptions;

namespace CryptocurrencyExchange.Core.Models
{
    public class WalletItem
    {
        public int Id { get; private set; }
        public int UserId { get; private set; }
        public User User { get; private set; }
        public CoinSymbol Symbol { get; private set; }
        public decimal Amount { get; private set; }

        private WalletItem() { } // for ORM

        public WalletItem(int userId, CoinSymbol symbol)
        {
            UserId = userId;
            Symbol = symbol;
            Amount = 0;
        }

        public void AddAmount(decimal amount)
        {
            Amount += amount;
        }
        public void RemoveAmount(decimal amount)
        {
            if (Amount < amount)
                throw new InsufficientFundsException(Symbol.Value);
            Amount -= amount;
        }
    }
}
