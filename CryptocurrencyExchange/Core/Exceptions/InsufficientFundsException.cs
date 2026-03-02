namespace CryptocurrencyExchange.Exceptions
{
    public class InsufficientFundsException : Exception
    {
        public InsufficientFundsException()
            : base("Not enough balance to perform this operation.")
        {
        }

        public InsufficientFundsException(string message)
            : base(message)
        {
        }

        public InsufficientFundsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
