namespace CryptocurrencyExchange.Exceptions
{
    public class EmailSendingFailedException : Exception
    {
        public EmailSendingFailedException(string email)
            : base($"Failed to send email to '{email}'.")
        {
        }

        public EmailSendingFailedException(string email, Exception innerException)
            : base($"Failed to send email to '{email}'.", innerException)
        {
        }
    }
}
