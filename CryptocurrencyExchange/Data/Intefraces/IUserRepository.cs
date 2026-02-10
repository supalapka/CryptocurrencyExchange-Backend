using CryptocurrencyExchange.Models;

namespace CryptocurrencyExchange.Data.Intefraces
{
    public interface IUserRepository
    {
        Task<bool> UserExists(string email);
        Task AddUserAsync(User user);
    }
}
