using CryptocurrencyExchange.Models;

namespace CryptocurrencyExchange.Services.Interfaces
{
    public interface IAuthDomainService
    {
        User CreateUser(string email, string password);
    }
}
