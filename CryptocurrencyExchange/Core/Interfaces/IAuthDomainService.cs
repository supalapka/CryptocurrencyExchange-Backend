using CryptocurrencyExchange.Core.Models;

namespace CryptocurrencyExchange.Services.Interfaces
{
    public interface IAuthDomainService
    {
        User CreateUser(string email, string password);
        bool VerifyPassword(string password, User user);
    }
}
