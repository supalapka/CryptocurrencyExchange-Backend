using CryptocurrencyExchange.Core.Entities;

namespace CryptocurrencyExchange.Core.Interfaces.Repositories
{
    public interface ICryptoNewsRepository
    {
        Task AddAsync(CryptoNews news);
    }
}
