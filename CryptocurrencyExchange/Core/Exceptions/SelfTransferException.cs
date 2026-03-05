namespace CryptocurrencyExchange.Exceptions
{
    public class SelfTransferException : Exception
    {
        public SelfTransferException()
            : base("Cannot transfer funds to yourself.")
        {
        }
    }
}
