using CryptocurrencyExchange.Core.Interfaces.Repositories;
using CryptocurrencyExchange.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace CryptocurrencyExchange.Infrastructure.Persistence.Repositories
{
    public class EfStakingRepository : IStakingRepository
    {
        private readonly DataContext _context;

        public EfStakingRepository(DataContext context)
        {
            _context = context;
        }


        public async Task AddAsync(Staking stakingData)
        {
            await _context.Staking.AddAsync(stakingData);
        }

        public async Task<List<Staking>> GetAllActiveStakingsAsync()
        {
            return await _context.Staking.Where(x => x.IsCompleted == false).ToListAsync();
        }

        public async Task<List<StakingCoin>> GetAllStakingCoinsAsync()
        {
            return await _context.StakingCoins.ToListAsync();
        }

        public async Task<StakingCoin?> GetCoinByIdAsync(int stakingId)
        {
            return await _context.StakingCoins.FindAsync(stakingId);
        }

        public async Task<Staking?> GetStakeDataByIdAsync(int stakedDataId)
        {
            return await _context.Staking.Include(s => s.StakingCoin).FirstAsync(x => x.Id == stakedDataId);
        }

        public async Task<List<Staking>> GetStakingsByUserAsync(int userId)
        {
            return await _context.Staking.Include(s => s.StakingCoin).Where(x => x.UserId == userId).ToListAsync();
        }
    }
}
