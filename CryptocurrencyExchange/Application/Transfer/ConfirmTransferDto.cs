namespace CryptocurrencyExchange.Application.Transfers
{
    public record ConfirmTransferDto(int TransferId, string VerificationCode);
}
