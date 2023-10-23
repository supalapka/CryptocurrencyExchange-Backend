using CryptocurrencyExchange.Data;
using CryptocurrencyExchange.Models;
using CryptocurrencyExchange.Utilities;
using Microsoft.EntityFrameworkCore;

namespace CryptocurrencyExchange.Services
{
    public class StakingService : IStakingService
    {
        private readonly DataContext _dataContext;

        public StakingService(DataContext context)
        {
            _dataContext = context;
        }


        public async Task CreateUserStaking(int userId, int stakingCoinId, float amount, int durationIdMonth)
        {
            var stakinCoin = _dataContext.StakingCoins.Find(stakingCoinId);

            var coinWalletItem = _dataContext.WalletItems.FirstOrDefault(x => x.UserId == userId
            && x.Symbol == stakinCoin.Symbol);
            if (coinWalletItem == null)
                throw new Exception($"Buy {stakinCoin.Symbol} first");

            if (coinWalletItem.Amount < amount)
                throw new Exception($"Not anough balance in {stakinCoin.Symbol}");

            coinWalletItem.Amount -= amount;
            coinWalletItem.Amount = (double)await UtilFunсtions.RoundCoinAmountUpTo1USD((decimal)coinWalletItem.Amount, coinWalletItem.Symbol);

            var stakingmodel = new Staking();
            stakingmodel.UserId = userId;
            stakingmodel.StakingCoinId = stakingCoinId;
            stakingmodel.Amount = amount;
            stakingmodel.DurationInMonth = durationIdMonth;
            stakingmodel.StartDate = DateTime.Today;
            stakingmodel.IsCompleted = false;

            await _dataContext.Staking.AddAsync(stakingmodel);
            await _dataContext.SaveChangesAsync();
        }


        public async Task CheckForExpiredStakings()
        {
            DateTime currentDateTime = DateTime.Now;

            var stakings = _dataContext.Staking.Where(x => x.IsCompleted == false).ToList();

            foreach (var staking in stakings)
            {
                DateTime stakingEndDate = staking.StartDate.AddDays(staking.DurationInMonth * 30);

                if (currentDateTime >= stakingEndDate)
                {
                    await PayStakingReward(staking.Id);
                    _dataContext.SaveChanges();
                }
                else
                {
                    // Период стейкинга еще не истек
                }
            }
        }



        public List<Staking> GetStakingsByUser(int userId)
        {
            return _dataContext.Staking
              .Include(s => s.StakingCoin)
              .Where(x => x.UserId == userId)
              .ToList();
        }


        public StakingCoin GetStakingCoinById(int id)
        {
            return _dataContext.StakingCoins.Find(id);
        }


        public async Task PayStakingReward(int stakingId)
        {
            var staking = _dataContext.Staking.Include(s => s.StakingCoin).First(x => x.Id == stakingId);
            if (staking.IsCompleted == true) return;
            var userWalletItem = _dataContext.WalletItems.First(x => x.UserId == staking.UserId && x.Symbol == staking.StakingCoin.Symbol);

            staking.IsCompleted = true;
            float persentageToAdd = staking.StakingCoin.RatePerMonth * staking.DurationInMonth;
            var coinsToAdd = staking.Amount;
            float rewards = (staking.Amount / 100) * persentageToAdd;
            coinsToAdd += rewards;
            // coinsToAdd = UtilFunstions.RoundCoinAmountUpTo1USD(coinsToAdd, coinPrice);
            userWalletItem.Amount += coinsToAdd;
            await _dataContext.SaveChangesAsync();
        }


        public List<StakingCoin> GetCoins()
        {
            return _dataContext.StakingCoins.ToList();
        }
    }
}
