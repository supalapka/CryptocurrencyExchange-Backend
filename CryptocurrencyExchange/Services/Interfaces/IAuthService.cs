using CryptocurrencyExchange.Models;

namespace CryptocurrencyExchange.Services.Interfaces
{
    public interface IAuthService
    {
        Task RegisterAsync(string email, string password);
        Task<string> LoginAsync(string email, string password);
        Task<string> GetEmailByIdAsync(int userId);
    }
}
