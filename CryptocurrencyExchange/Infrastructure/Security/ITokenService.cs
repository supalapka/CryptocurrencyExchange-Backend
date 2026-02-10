using CryptocurrencyExchange.Core.Models;

namespace CryptocurrencyExchange.Infrastructure.Security
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
