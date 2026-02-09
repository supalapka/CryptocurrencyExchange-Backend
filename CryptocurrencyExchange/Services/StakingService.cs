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

            foreach (var staking in stakings)
            {
                DateTime stakingEndDate = staking.StartDate.AddDays(staking.DurationInMonth * 30);

                if (currentDateTime >= stakingEndDate)
                {
                    await PayStakingReward(staking.Id);
                    // _dataContext.SaveChanges();
                }
                else
                {
                    // Период стейкинга еще не истек
                }
            }
        }


        public async Task<List<Staking>> GetStakingsByUser(int userId)
        {
            return await _stakingRepository.GetStakingsByUserAsync(userId);
        }


        public async Task PayStakingReward(int stakingId)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                Staking stakingData = await _stakingRepository.GetStakeDataByIdAsync(stakingId)
                    ?? throw new Exception("Staking data not found, Id: " + stakingId.ToString());

                WalletItem userWalletItem = await _walletItemRepository.GetAsync(stakingData.UserId, stakingData.StakingCoin.Symbol)
                    ?? throw new WalletItemNotFoundException($"staking coin not found: {stakingData.StakingCoin.Symbol} in wallet");

                {
                    if (stakingData.IsCompleted == true)
                        return;

                    stakingData.IsCompleted = true;

                    float persentageToAdd = stakingData.StakingCoin.RatePerMonth * stakingData.DurationInMonth;
                    var coinsToAdd = stakingData.Amount;
                    decimal rewards = (stakingData.Amount / 100) * (decimal)persentageToAdd;
                    coinsToAdd += rewards;
                    userWalletItem.Amount += coinsToAdd;
                }
            });
        }


        public async Task<List<StakingCoin>> GetCoinsAsync()
        {
            return await _stakingRepository.GetAllStakingCoinsAsync();
        }

    }
}
