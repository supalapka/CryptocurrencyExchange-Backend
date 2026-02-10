using CryptocurrencyExchange.Core.Models;

namespace CryptocurrencyExchange.Services.StakingServices
{
    public interface IStakingService
    {
        Task CreateUserStaking(int userId, int stakingCoinId, decimal amount, int durationInDays);
        Task<List<Staking>> GetStakingsByUser(int userId);
        Task<List<StakingCoin>> GetCoinsAsync();
        Task CheckForExpiredStakings();
        Task PayStakingReward(Staking stakingData);

    }
}
