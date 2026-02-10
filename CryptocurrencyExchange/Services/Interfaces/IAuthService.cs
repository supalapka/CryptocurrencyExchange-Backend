using CryptocurrencyExchange.Models;

namespace CryptocurrencyExchange.Services.Interfaces
{
    public interface IAuthService
    {
        Task RegisterAsync(string email, string password);
    }
}
