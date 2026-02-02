using CryptocurrencyExchange.Models;

namespace CryptocurrencyExchange.Services
{
    public interface IFuturesService
    {
        Task<int> CreateFutureAsync(FutureDto futureDto, int userId);
        List<FutureDto> GetFuturePositions(int userId);
        Task LiquidatePosition(int id, double markPrice);
        Task ClosePosition(int id, double pnl, double makrPrice);
        Task<List<FutureHIstoryOutput>> GetHistoryAsync(int userId, int page);

    }
}
