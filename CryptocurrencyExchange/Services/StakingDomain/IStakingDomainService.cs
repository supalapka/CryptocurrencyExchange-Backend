using CryptocurrencyExchange.Models;

namespace CryptocurrencyExchange.Services.StakingDomain
{
    public interface IStakingDomainService
    {
        Staking CreateStaking(WalletItem stakingCoinWalletItem, StakingCoin coinToStake, decimal amountToStake, int durationInMonth);
    }
}
