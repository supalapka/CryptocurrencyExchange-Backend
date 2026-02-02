namespace CryptocurrencyExchange.Exceptions
{
    public class WalletItemNotFoundException : Exception
    {
        public WalletItemNotFoundException()
          : base("Wallet item not found.")
        {
        }

        public WalletItemNotFoundException(string message)
            : base(message)
        {
        }

        public WalletItemNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
