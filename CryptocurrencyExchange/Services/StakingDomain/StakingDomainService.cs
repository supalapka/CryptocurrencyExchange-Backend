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
    }
}
