using CryptocurrencyExchange.Exceptions;
using CryptocurrencyExchange.Models;
using CryptocurrencyExchange.Services.Interfaces;
using CryptocurrencyExchange.Services.StakingDomain;

namespace CryptocurrencyExchange.Services
{
    public class StakingService : IStakingService
    {
        private readonly IStakingRepository _stakingRepository;
        private readonly IWalletItemRepository _walletItemRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStakingDomainService _stakingDomainService;

        public StakingService(
            IStakingRepository stakingRepository,
            IWalletItemRepository walletItemRepository,
            IUnitOfWork unitOfWork,
            IStakingDomainService stakingDomainService
            )
        {
            _stakingRepository = stakingRepository;
            _walletItemRepository = walletItemRepository;
            _unitOfWork = unitOfWork;
            _stakingDomainService = stakingDomainService;
        }


        public async Task CreateUserStaking(int userId, int stakingCoinId, decimal amount, int durationInMonth)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
             {
                 StakingCoin stakingCoin = await _stakingRepository.GetCoinByIdAsync(stakingCoinId)
                      ?? throw new StakingCoinNotFoundException(stakingCoinId.ToString());

                 WalletItem coinWalletItem = await _walletItemRepository.GetAsync(userId, stakingCoin.Symbol)
                      ?? throw new WalletItemNotFoundException($"staking coin not found: {stakingCoin.Symbol} in wallet");

                 Staking stakingData = _stakingDomainService.CreateStaking(coinWalletItem, stakingCoin, amount, durationInMonth);

                 await _stakingRepository.AddAsync(stakingData);
             });
        }


        public async Task CheckForExpiredStakings()
        {
            DateTime currentDateTime = DateTime.Now;

            List<Staking> stakings = await _stakingRepository.GetAllActiveStakingsAsync();

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                foreach (var stakingData in stakings)
                {
                    if (_stakingDomainService.IsExpired(stakingData))
                    {
                        await PayStakingReward(stakingData);
                    }
                    else
                    {
                        // The staking period has not ended yet
                    }
                }
            });
        }


        public async Task PayStakingReward(Staking stakingData)
        {
            WalletItem stakedCoinWalletItem = await _walletItemRepository.GetAsync(stakingData.UserId, stakingData.StakingCoin.Symbol)
                ?? throw new WalletItemNotFoundException($"staking coin not found: {stakingData.StakingCoin.Symbol} in wallet");

            _stakingDomainService.CompleteStaking(stakingData, stakedCoinWalletItem);
        }


        public async Task<List<Staking>> GetStakingsByUser(int userId)
        {
            return await _stakingRepository.GetStakingsByUserAsync(userId);
        }

        public async Task<List<StakingCoin>> GetCoinsAsync()
        {
            return await _stakingRepository.GetAllStakingCoinsAsync();
        }

    }
}
