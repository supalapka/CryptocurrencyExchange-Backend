using CryptocurrencyExchange.Application.ApiKeys;

namespace CryptocurrencyExchange.Core.Interfaces.Services
{
    public interface IApiKeyService
    {
        Task<string> GenerateAsync(int userId);
        Task<ApiKeyDto> GetAsync(int userId);
    }
}
