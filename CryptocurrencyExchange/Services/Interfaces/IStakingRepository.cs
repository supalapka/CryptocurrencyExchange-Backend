using CryptocurrencyExchange.Models;

namespace CryptocurrencyExchange.Services.Interfaces
{
    public interface IStakingRepository
    {
        Task AddAsync(Staking stakingData);
        Task<StakingCoin?> GetCoinByIdAsync(int stakingId);
        Task<Staking?> GetStakeDataByIdAsync(int stakedDataId);
        Task<List<Staking>> GetStakingsByUserAsync(int userId);
        Task <List<StakingCoin>> GetAllStakingCoinsAsync();
        Task<List<Staking>> GetAllActiveStakingsAsync();
    }
}
