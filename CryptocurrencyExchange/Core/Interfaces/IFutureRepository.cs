using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Services.Futures;

namespace CryptocurrencyExchange.Core.Interfaces
{
    public interface IFutureRepository
    {
        Task AddAsync(Future future);
        Task<Future?> GetByIdAsync(int id);
        Task<List<Future>> GetCompletedFuturesAsync(int userId);
        Task AddPositionToHistoryAsync(Future future, double markPrice);
        Task<List<FutureHIstoryOutput>> GetHistoryAsync(int userId, int page, int positionsPerPage);
    }

}
