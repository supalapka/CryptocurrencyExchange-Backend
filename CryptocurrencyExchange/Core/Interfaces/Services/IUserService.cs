namespace CryptocurrencyExchange.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<string> GetEmailByIdAsync(int userId);
    }
}
