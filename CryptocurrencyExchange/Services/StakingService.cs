using CryptocurrencyExchange.Data;
using CryptocurrencyExchange.Models;

namespace CryptocurrencyExchange.Services
{
    public class StakingService : IStakingService
    {
        private readonly DataContext _dataContext;

        public StakingService(DataContext context)
        {
            _dataContext = context;
        }


        public async Task CreateStakingCoin(int userId, int stakingCoinId, float amount, int durationIdDays)
        {
            var stakinCoin = _dataContext.StakingCoins.Find(stakingCoinId);

            var coinWalletItem = _dataContext.WalletItems.Where(x => x.UserId == userId
            && x.Symbol == stakinCoin.Symbol).FirstOrDefault();
            if(coinWalletItem == null)
                throw new Exception($"Buy {stakinCoin.Symbol} first");

            if (coinWalletItem.Amount < amount)
                throw new Exception($"Not anough balance in {stakinCoin.Symbol}");

            coinWalletItem.Amount -= amount;

            var stakingmodel = new Staking();
            stakingmodel.UserId = userId;
            stakingmodel.StakingCoinId = stakingCoinId;
            stakingmodel.Amount = amount;
            stakingmodel.DurationInDays = durationIdDays;
            stakingmodel.StartDate = DateTime.Today;
            stakingmodel.IsCompleted = false;

            await _dataContext.Staking.AddAsync(stakingmodel);
            await _dataContext.SaveChangesAsync();
        }


        public Task CheckForExpiredStaking()
        {
            throw new NotImplementedException();
        }


        public Task CreateStakingCoin(int userId, string symbol, int durationIdDays)
        {
            throw new NotImplementedException();
        }


        public Task GetAllStakingCoinsForUser(int userId)
        {
            throw new NotImplementedException();
        }


        public Task GetStakingCoinById(int id)
        {
            throw new NotImplementedException();
        }


        public Task PayStakingReward(int stakingId)
        {
            throw new NotImplementedException();
        }

    }
}
