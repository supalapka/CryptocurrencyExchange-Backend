using CryptocurrencyExchange.Core.Models;

namespace CryptocurrencyExchange.Core.Interfaces
{
    public interface IStakingDomainService
    {
        Staking CreateStaking(WalletItem stakingCoinWalletItem, StakingCoin coinToStake, decimal amountToStake, int durationInMonth);
        bool IsExpired(Staking staking);
        void CompleteStaking(Staking stakingData, WalletItem coinWalletItem);
    }
}
