using CryptocurrencyExchange.Exceptions;
using CryptocurrencyExchange.Models;
using CryptocurrencyExchange.Services.Interfaces;
using CryptocurrencyExchange.Services.Wallet;
using Microsoft.EntityFrameworkCore;

namespace CryptocurrencyExchange.Services
{
    public class WalletService : IWalletService
    {
        private readonly IMarketService _marketService;
        private readonly IWalletItemRepository _walletItemRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWalletDomainService _walletDomainService;

        public WalletService(
            INotificationService notificatinService,
            IMarketService marketService,
            IWalletItemRepository walletItemRepository,
            IUnitOfWork unitOfWork,
            IWalletDomainService walletDomainService)
        {
            _marketService = marketService;
            _walletItemRepository = walletItemRepository;
            _unitOfWork = unitOfWork;
            _walletDomainService = walletDomainService;
        }


        public async Task BuyAsync(int userId, string coinSymbol, decimal usd)
        {
            coinSymbol = coinSymbol.ToLower();

            var usdt = await _walletItemRepository.GetAsync(userId, "usdt")
            ?? throw new WalletItemNotFoundException("USDT in wallet not found");

            if ((decimal)usdt.Amount < usd)
                throw new InsufficientFundsException();

            var coin = await _walletItemRepository.GetAsync(userId, coinSymbol)
             ?? throw new WalletItemNotFoundException($"{coinSymbol}in wallet not found");

            var coinPrice = await _marketService.GetPrice(coinSymbol);

            _walletDomainService.Buy(usdt, coin, usd, coinPrice);

            await _unitOfWork.CommitAsync();
        }


        public async Task<double> GetCoinAmountAsync(int userId, string symbol)
        {
            var walletItem = await _walletItemRepository.GetAsync(userId, symbol);
            return walletItem?.Amount ?? 0;
        }


        public async Task<List<WalletItem>> GetFullWalletAsync(int userId)
        {
            return await _walletItemRepository.GetNonEmptyByUserAsync(userId);
        }


        public async Task<WalletItem> GetOrCreateWalletItem(int userId, string symbol)
        {
            var item = await _walletItemRepository.GetAsync(userId, symbol);

            if (item == null)
            {
                item = new WalletItem()
                {
                    Symbol = symbol,
                    Amount = 0,
                    UserId = userId
                };
                await _walletItemRepository.AddAsync(item);
            }

            return item;
        }


        public async Task SellAsync(int userId, string coinSymbol, double amount)
        {
            coinSymbol = coinSymbol.ToLower();

            var coin = await _walletItemRepository.GetAsync(userId, coinSymbol)
             ?? throw new WalletItemNotFoundException($"{coinSymbol} wallet not found");

            var usdt = await _walletItemRepository.GetAsync(userId, "usdt")
              ?? throw new WalletItemNotFoundException("USDT wallet not found");

            if (coin.Amount < amount)
                throw new InsufficientFundsException($"Not enough balance in {coinSymbol.ToUpper()}");

            var coinPrice = (decimal)await _marketService.GetPrice(coinSymbol);
            _walletDomainService.Sell(usdt, coin, (decimal)amount, coinPrice);
            await _unitOfWork.CommitAsync();
        }
    }
}
