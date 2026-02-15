using CryptocurrencyExchange.Core.Domain.Wallet;
using CryptocurrencyExchange.Core.Interfaces;
using CryptocurrencyExchange.Core.Interfaces.Repositories;
using CryptocurrencyExchange.Core.Interfaces.Services;
using CryptocurrencyExchange.Core.Models;

namespace CryptocurrencyExchange.Services.Wallets
{
    public class WalletService : IWalletService
    {
        private readonly IMarketService _marketService;
        private readonly IWalletItemRepository _walletItemRepository;
        private readonly IUnitOfWork _unitOfWork;

        public WalletService(
            IMarketService marketService,
            IWalletItemRepository walletItemRepository,
            IUnitOfWork unitOfWork
            )
        {
            _marketService = marketService;
            _walletItemRepository = walletItemRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task BuyAsync(int userId, string coinSymbol, decimal usd)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                coinSymbol = coinSymbol.ToLower();
                var tradeCoinPrice = await _marketService.GetPrice(coinSymbol);

                var tradeItems = await _walletItemRepository.GetCoinsDataForTradeAsync(userId, coinSymbol);
                IEnumerable<WalletItem> walletItemsToTrade = new[] { tradeItems.BaseCurrency, tradeItems.TradedCurrency };

                Wallet wallet = new Wallet(userId, walletItemsToTrade);
                wallet.Buy(coinSymbol, usd, tradeCoinPrice);
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
                IEnumerable<WalletItem> walletItemsToTrade = new[] { tradeItems.BaseCurrency, tradeItems.TradedCurrency };

                Wallet wallet = new Wallet(userId, walletItemsToTrade);
                wallet.Sell(coinSymbol, amount, tradeCoinPrice);
            });
        }
    }
}
