namespace CryptocurrencyExchange.Application.Transfers
{
    public record InitiateTransferDto(string ReceiverEmail, string CoinSymbol, decimal Amount);
}
