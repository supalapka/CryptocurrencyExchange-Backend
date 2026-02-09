using CryptocurrencyExchange.Exceptions;
using CryptocurrencyExchange.Models;

namespace CryptocurrencyExchange.Services.StakingDomain
{
    public class StakingDomainService : IStakingDomainService
    {
        public Staking CreateStaking(WalletItem stakingCoinWalletItem, StakingCoin cointToStake, decimal amountToStake, int durationInMonth)
        {
            if (stakingCoinWalletItem.Amount < amountToStake)
                throw new InsufficientFundsException($"Not anough balance in {stakingCoinWalletItem.Symbol} to stake");

            stakingCoinWalletItem.Amount -= amountToStake;

            var stakingData = new Staking()
            {
                UserId = stakingCoinWalletItem.UserId,
                StakingCoinId = cointToStake.Id,
                Amount = amountToStake,
                DurationInMonth = durationInMonth,
                StartDate = DateTime.Today,
                IsCompleted = false
            };

            return stakingData;
        }

        public void CompleteStaking(Staking stakingData, WalletItem coinWalletItem)
        {
            if (stakingData.IsCompleted == true)
                return;

            stakingData.IsCompleted = true;

            float persentageToAdd = stakingData.StakingCoin.RatePerMonth * stakingData.DurationInMonth;
            var coinsToAdd = stakingData.Amount;
            decimal rewards = (stakingData.Amount / 100) * (decimal)persentageToAdd;
            coinsToAdd += rewards;
            coinWalletItem.Amount += coinsToAdd;
        }

        public bool IsExpired(Staking staking)
        {
            DateTime stakingEndDate = staking.StartDate.AddDays(staking.DurationInMonth * 30);
            return DateTime.Now >= stakingEndDate;
        }

    }
}
