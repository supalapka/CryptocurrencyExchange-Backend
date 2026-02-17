namespace CryptocurrencyExchange.Exceptions
{
    public class InvalidEmailException : Exception
    {
        public InvalidEmailException()
            : base("The email is invalid or empty.") { }

        public InvalidEmailException(string email)
            : base($"The email '{email}' is invalid or empty.") { }

        public InvalidEmailException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}