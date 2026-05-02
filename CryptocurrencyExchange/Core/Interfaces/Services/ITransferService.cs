using CryptocurrencyExchange.Application.Transfers;

namespace CryptocurrencyExchange.Core.Interfaces.Services
{
    public interface ITransferService
    {
        Task<int> InitiateAsync(int senderId, InitiateTransferDto dto, string idempotencyKey);
        Task ConfirmAsync(int senderId, ConfirmTransferDto dto);
    }
}
