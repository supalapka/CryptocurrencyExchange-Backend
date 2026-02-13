using CryptocurrencyExchange.Core.Interfaces;
using CryptocurrencyExchange.Core.Interfaces.Services;
using CryptocurrencyExchange.Core.Models;

namespace CryptocurrencyExchange.Services.Wallet
{
    public class WalletService : IWalletService
    {
        private readonly IMarketService _marketService;
        private readonly IWalletItemRepository _walletItemRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWalletDomainService _walletDomainService;

        public WalletService(
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
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                coinSymbol = coinSymbol.ToLower();
                var tradeCoinPrice = await _marketService.GetPrice(coinSymbol);

                var tradeItems = await _walletItemRepository.GetCoinsDataForTradeAsync(userId, coinSymbol);

                _walletDomainService.Buy(tradeItems.BaseCurrency, tradeItems.TradedCurrency, usd, tradeCoinPrice);
            });
        }


        public async Task<decimal> GetCoinAmountAsync(int userId, string symbol)
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


        public async Task SellAsync(int userId, string coinSymbol, decimal amount)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                coinSymbol = coinSymbol.ToLower();
                var tradeCoinPrice = await _marketService.GetPrice(coinSymbol);

                var tradeItems = await _walletItemRepository.GetCoinsDataForTradeAsync(userId, coinSymbol);

                _walletDomainService.Sell(tradeItems.BaseCurrency, tradeItems.TradedCurrency, amount, tradeCoinPrice);
            });
        }
    }
}
