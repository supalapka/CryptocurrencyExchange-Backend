using CryptocurrencyExchange.Core.Models;

namespace CryptocurrencyExchange.Core.Interfaces.Repositories
{
    public interface IApiKeyRepository
    {
        Task<ApiKey?> GetByHashAsync(byte[] hash);
        Task AddAsync(ApiKey apiKey);
        Task<ApiKey?> GetByUserIdAsync(int userId);
        Task DeleteAsync(ApiKey apiKey);
    }
}
