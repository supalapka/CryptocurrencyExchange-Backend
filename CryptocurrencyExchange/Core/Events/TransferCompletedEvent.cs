namespace CryptocurrencyExchange.Core.Events
{
    public record TransferCompletedEvent(
        int TransferId,
        int SenderId,
        string SenderEmail,
        int ReceiverId,
        string ReceiverEmail,
        decimal Amount,
        string Symbol
    );
}
