using CryptocurrencyExchange.Core.Interfaces;
using CryptocurrencyExchange.Core.Interfaces.Repositories;
using CryptocurrencyExchange.Core.Interfaces.Services;
using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Exceptions;

namespace CryptocurrencyExchange.Application.StakingServices
{
    public class StakingService : IStakingService
    {
        private readonly IStakingRepository _stakingRepository;
        private readonly IWalletItemRepository _walletItemRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStakingDomainService _stakingDomainService;
        private readonly ILogger<StakingService> _logger;

        public StakingService(
            IStakingRepository stakingRepository,
            IWalletItemRepository walletItemRepository,
            IUnitOfWork unitOfWork,
            IStakingDomainService stakingDomainService,
            ILogger<StakingService> logger)
        {
            _stakingRepository = stakingRepository;
            _walletItemRepository = walletItemRepository;
            _unitOfWork = unitOfWork;
            _stakingDomainService = stakingDomainService;
            _logger = logger;
        }


        public async Task CreateUserStaking(int userId, int stakingCoinId, decimal amount, int durationInMonth)
        {
            throw new NotImplementedException();
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
             {
                 StakingCoin stakingCoin = await _stakingRepository.GetCoinByIdAsync(stakingCoinId)
                      ?? throw new StakingCoinNotFoundException(stakingCoinId.ToString());
             });
        }


        public async Task CheckForExpiredStakings()
        {
            DateTime currentDateTime = DateTime.Now;

            List<Staking> stakings = await _stakingRepository.GetAllActiveStakingsAsync();

            _logger.LogInformation("Checking {Count} active stakings for expiry", stakings.Count);

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                foreach (var stakingData in stakings)
                {
                    if (_stakingDomainService.IsExpired(stakingData))
                    {
                        _logger.LogInformation("Staking {StakingId} for user {UserId} has expired, paying reward",
                            stakingData.Id, stakingData.UserId);
                        await PayStakingReward(stakingData);
                    }
                }
            });
        }


        public async Task PayStakingReward(Staking stakingData)
        {
            throw new NotImplementedException();
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
