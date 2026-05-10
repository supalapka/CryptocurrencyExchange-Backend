namespace CryptocurrencyExchange.Exceptions
{
    public class ApiKeyNotFoundException : Exception
    {
        public ApiKeyNotFoundException()
            : base("API key not found.")
        {
        }
    }
}
