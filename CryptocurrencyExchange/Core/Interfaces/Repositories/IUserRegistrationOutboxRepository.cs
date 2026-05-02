using CryptocurrencyExchange.Core.Models;

namespace CryptocurrencyExchange.Core.Interfaces.Repositories
{
    public interface IUserRegistrationOutboxRepository
    {
        Task AddAsync(UserRegistrationOutbox entry);
        Task<List<UserRegistrationOutbox>> GetPendingAsync();
    }
}
