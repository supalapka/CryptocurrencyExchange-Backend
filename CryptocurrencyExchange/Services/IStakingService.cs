using CryptocurrencyExchange.Models;

namespace CryptocurrencyExchange.Services
{
    public interface IStakingService
    {
        Task CreateUserStaking(int userId, int stakingCoinId, decimal amount, int durationIdDays);
        List<Staking> GetStakingsByUser(int userId);
        StakingCoin GetStakingCoinById(int id);
        List<StakingCoin> GetCoins();
        Task CheckForExpiredStakings();
        Task PayStakingReward(int stakingId);

    }
}
