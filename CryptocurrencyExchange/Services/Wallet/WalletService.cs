using CryptocurrencyExchange.Core.Domain.Wallet.Commands;
using CryptocurrencyExchange.Core.Domain.Wallets;
using CryptocurrencyExchange.Core.Interfaces;
using CryptocurrencyExchange.Core.Interfaces.Repositories;
using CryptocurrencyExchange.Core.Interfaces.Services;
using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Core.ValueObject;
using CryptocurrencyExchange.Services.WalletTrade;

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

        public async Task BuyAsync(int userId, CoinTradeDto coinTradeDto)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                IEnumerable<WalletItem> walletItemsToTrade = await GetWalletTradeItems(userId, coinTradeDto);
                WalletTradeCommand walletTradeCommand = await CreateWalletTradeCommand(userId, coinTradeDto);

                Wallet wallet = new Wallet(userId, walletItemsToTrade);
                wallet.Buy(walletTradeCommand);
            });
        }

        public async Task<decimal> GetCoinAmountAsync(int userId, CoinSymbol symbol)
        {
            var walletItem = await _walletItemRepository.GetAsync(userId, symbol);

            return walletItem?.Amount ?? Balance.Zero;
        }

        public async Task<List<WalletItem>> GetFullWalletAsync(int userId)
        {
            return await _walletItemRepository.GetNonEmptyByUserAsync(userId);
        }

        public async Task<WalletItem> GetOrCreateWalletItem(int userId, CoinSymbol symbol)
        {
            var item = await _walletItemRepository.GetAsync(userId, symbol);

            if (item == null)
            {
                item = new WalletItem(userId, symbol);
                await _walletItemRepository.AddAsync(item);
            }

            return item;
        }

        public async Task SellAsync(int userId, CoinTradeDto coinTradeDto)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                IEnumerable<WalletItem> walletItemsToTrade = await GetWalletTradeItems(userId, coinTradeDto);
                WalletTradeCommand walletTradeCommand = await CreateWalletTradeCommand(userId, coinTradeDto);

                Wallet wallet = new Wallet(userId, walletItemsToTrade);
                wallet.Sell(walletTradeCommand);
            });
        }

        private async Task<IEnumerable<WalletItem>> GetWalletTradeItems(int userId, CoinTradeDto coinTradeDto)
        {
            var tradeItems = await _walletItemRepository.GetCoinsDataForTradeAsync(userId, coinTradeDto.CoinSymbol);
            return new[] { tradeItems.BaseCurrency, tradeItems.TradedCurrency };
        }

        private async Task<WalletTradeCommand> CreateWalletTradeCommand(int userId, CoinTradeDto coinTradeDto)
        {
            var tradeCoinPrice = await _marketService.GetPrice(coinTradeDto.CoinSymbol.Value);
            return new WalletTradeCommand(coinTradeDto.CoinSymbol, coinTradeDto.CoinAmount, tradeCoinPrice);
        }
    }
}
