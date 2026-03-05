namespace CryptocurrencyExchange.Exceptions
{
    public class TransferNotFoundException : Exception
    {
        public TransferNotFoundException()
            : base("Transfer not found.")
        {
        }
    }
}
