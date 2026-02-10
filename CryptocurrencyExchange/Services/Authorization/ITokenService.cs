using CryptocurrencyExchange.Models;

namespace CryptocurrencyExchange.Services.Authorization
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
