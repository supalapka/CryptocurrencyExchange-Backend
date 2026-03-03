using CryptocurrencyExchange.Core.ValueObject.User;

namespace CryptocurrencyExchange.Core.Interfaces.Services
{
    public interface IAuthService
    {
        Task RegisterAsync(Email email, Password password);
        Task<string> LoginAsync(Email email, Password password);
    }
}
