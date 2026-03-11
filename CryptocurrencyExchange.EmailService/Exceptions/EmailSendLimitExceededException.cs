namespace CryptocurrencyExchange.EmailService.Exceptions
{
    public class EmailSendLimitExceededException : Exception
    {
        public EmailSendLimitExceededException(int dailyLimit)
            : base($"Daily email limit of {dailyLimit} reached. Switch to a dedicated email provider (SendGrid, AWS SES, Mailgun) to send more.")
        {
        }
    }
}
