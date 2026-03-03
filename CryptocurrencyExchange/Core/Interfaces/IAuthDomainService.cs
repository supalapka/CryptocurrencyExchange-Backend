using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Core.ValueObject.User;

namespace CryptocurrencyExchange.Core.Interfaces
{
    public interface IAuthDomainService
    {
        User CreateUser(Email email, Password password);
        bool VerifyPassword(Password password, User user);
    }
}
