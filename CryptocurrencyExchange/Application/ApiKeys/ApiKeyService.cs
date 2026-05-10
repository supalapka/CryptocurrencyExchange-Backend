using CryptocurrencyExchange.Core.Interfaces;
using CryptocurrencyExchange.Core.Interfaces.Repositories;
using CryptocurrencyExchange.Core.Interfaces.Services;
using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Exceptions;

namespace CryptocurrencyExchange.Application.ApiKeys
{
    public class ApiKeyService : IApiKeyService
    {
        private readonly IApiKeyRepository _apiKeyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ApiKeyService(IApiKeyRepository apiKeyRepository, IUnitOfWork unitOfWork)
        {
            _apiKeyRepository = apiKeyRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> GenerateAsync(int userId)
        {
            var existing = await _apiKeyRepository.GetByUserIdAsync(userId);
            if (existing != null)
                await _apiKeyRepository.DeleteAsync(existing);

            var (entity, rawKey) = ApiKey.Create(userId);
            await _apiKeyRepository.AddAsync(entity);
            await _unitOfWork.CommitAsync();
            return rawKey;
        }

        public async Task<ApiKeyDto> GetAsync(int userId)
        {
            var apiKey = await _apiKeyRepository.GetByUserIdAsync(userId);
            if (apiKey == null)
                throw new ApiKeyNotFoundException();

            return new ApiKeyDto(apiKey.KeyPrefix, apiKey.CreatedAt);
        }
    }
}
