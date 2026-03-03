namespace CryptocurrencyExchange.Exceptions
{
    public class InvalidPasswordException : Exception
    {
        public InvalidPasswordException()
            : base("Invalid password")
        {
        }
    }
}
