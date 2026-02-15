using CryptocurrencyExchange.Exceptions;

namespace CryptocurrencyExchange.Core.Models
{
    public class WalletItem
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public decimal Amount { get; private set; }

        private WalletItem() { } // for ORM

        public WalletItem(int userId, string symbol)
        {
            UserId = userId;
            Symbol = symbol.ToLower();
            Amount = 0;
        }

        public void AddAmount(decimal amount)
        {
            Amount += amount;
        }
        public void RemoveAmount(decimal amount)
        {
            if (Amount < amount)
                throw new InsufficientFundsException(Symbol);
            Amount -= amount;
        }
    }
}
