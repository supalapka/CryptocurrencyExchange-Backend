namespace CryptocurrencyExchange.Exceptions
{
    public class InvalidVerificationCodeException : Exception
    {
        public InvalidVerificationCodeException()
            : base("The verification code is invalid.")
        {
        }
    }
}
