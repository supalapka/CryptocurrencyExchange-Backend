using CryptocurrencyExchange.Core.Models;

namespace CryptocurrencyExchange.Core.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<bool> UserExists(string email);
        Task<User> GetByEmailAsync(string email);
        Task<string> GetEmailByIdAsync(int userId);
        Task AddUserAsync(User user);
    }
}
