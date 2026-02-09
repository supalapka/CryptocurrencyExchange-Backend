using CryptocurrencyExchange.Models;

namespace CryptocurrencyExchange.Services.StakingDomain
{
    public interface IStakingDomainService
    {
        Staking CreateStaking(WalletItem stakingCoinWalletItem, StakingCoin coinToStake, decimal amountToStake, int durationInMonth);
        bool IsExpired(Staking staking);
        void CompleteStaking(Staking stakingData, WalletItem coinWalletItem);
    }
}
