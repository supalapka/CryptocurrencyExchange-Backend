namespace CryptocurrencyExchange.Services
{
    public interface IStakingService
    {
        Task CreateStakingCoin(int userId, int stakingCoinId, float amount, int durationIdDays);
        Task GetAllStakingCoinsForUser(int userId);
        Task GetStakingCoinById(int id);
        Task CheckForExpiredStaking();
        Task PayStakingReward(int stakingId);

    }
}
