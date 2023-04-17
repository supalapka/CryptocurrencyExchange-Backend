using CryptocurrencyExchange.Models;

namespace CryptocurrencyExchange.Services
{
    public interface IFuturesService
    {
        Task CreateFutureAsync(FutureDto futureDto, int userId);
        List<FutureDto> GetFuturePositions(int userId);
    }
}
