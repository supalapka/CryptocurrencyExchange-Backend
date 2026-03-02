using CryptocurrencyExchange.Core.Models;

namespace CryptocurrencyExchange.Core.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
