namespace CryptocurrencyExchange.Core.Events
{
    public record SendVerificationEmailCommand(string Email, string VerificationCode);
}
