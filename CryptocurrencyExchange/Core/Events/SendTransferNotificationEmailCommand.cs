namespace CryptocurrencyExchange.Core.Events
{
    public record SendTransferNotificationEmailCommand(int OutboxEntryId, string Email, string Subject, string Body);
}
