using CryptocurrencyExchange.Core.Interfaces.Repositories;
using CryptocurrencyExchange.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace CryptocurrencyExchange.Infrastructure.Persistence.Repositories
{
    public class EfApiKeyRepository : IApiKeyRepository
    {
        private readonly DataContext _dataContext;

        public EfApiKeyRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<ApiKey?> GetByHashAsync(byte[] hash)
        {
            return await _dataContext.ApiKeys.FirstOrDefaultAsync(k => k.KeyHash == hash);
        }

        public async Task<ApiKey?> GetByUserIdAsync(int userId)
        {
            return await _dataContext.ApiKeys.FirstOrDefaultAsync(k => k.UserId == userId);
        }

        public async Task AddAsync(ApiKey apiKey)
        {
            await _dataContext.ApiKeys.AddAsync(apiKey);
        }

        public Task DeleteAsync(ApiKey apiKey)
        {
            _dataContext.ApiKeys.Remove(apiKey);
            return Task.CompletedTask;
        }
    }
}
