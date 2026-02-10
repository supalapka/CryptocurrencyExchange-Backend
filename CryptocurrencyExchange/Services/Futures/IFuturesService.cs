namespace CryptocurrencyExchange.Services.Futures
{
    public interface IFuturesService
    {
        Task<int> CreateFutureAsync(FutureDto futureDto, int userId);
        Task<List<FutureDto>> GetFuturePositions(int userId);
        Task LiquidatePosition(int id, double markPrice);
        Task ClosePosition(int id, decimal pnl, double makrPrice);
        Task<List<FutureHIstoryOutput>> GetHistoryAsync(int userId, int page);

    }
}
