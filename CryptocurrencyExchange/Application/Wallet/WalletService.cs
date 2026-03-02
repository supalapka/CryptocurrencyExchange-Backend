using CryptocurrencyExchange.Core.Domain.Wallet.Commands;
using CryptocurrencyExchange.Core.Interfaces;
using CryptocurrencyExchange.Core.Interfaces.Repositories;
using CryptocurrencyExchange.Core.Interfaces.Services;
using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Core.ValueObject;
using CryptocurrencyExchange.Application.Wallet;
using WalletAggregate = CryptocurrencyExchange.Core.Domain.Wallets.Wallet;

namespace CryptocurrencyExchange.Application.Wallets
{
    public class WalletService : IWalletService
    {
        private readonly IMarketService _marketService;
        private readonly IWalletItemRepository _walletItemRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<WalletService> _logger;

        public WalletService(
            IMarketService marketService,
            IWalletItemRepository walletItemRepository,
            IUnitOfWork unitOfWork,
            ILogger<WalletService> logger)
        {
            _marketService = marketService;
            _walletItemRepository = walletItemRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task BuyAsync(int userId, CoinTradeDto coinTradeDto)
        {
            _logger.LogInformation("User {UserId} buying {Amount} {Symbol}",
                userId, coinTradeDto.CoinAmount, coinTradeDto.CoinSymbol);

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                IEnumerable<WalletItem> walletItemsToTrade = await GetWalletTradeItems(userId, coinTradeDto);
                WalletTradeCommand walletTradeCommand = await CreateWalletTradeCommand(userId, coinTradeDto);

                WalletAggregate wallet = new WalletAggregate(userId, walletItemsToTrade);
                wallet.Buy(walletTradeCommand);
            });

            _logger.LogInformation("User {UserId} successfully bought {Amount} {Symbol}",
                userId, coinTradeDto.CoinAmount, coinTradeDto.CoinSymbol);
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
            _logger.LogInformation("User {UserId} selling {Amount} {Symbol}",
                userId, coinTradeDto.CoinAmount, coinTradeDto.CoinSymbol);

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                IEnumerable<WalletItem> walletItemsToTrade = await GetWalletTradeItems(userId, coinTradeDto);
                WalletTradeCommand walletTradeCommand = await CreateWalletTradeCommand(userId, coinTradeDto);

                WalletAggregate wallet = new WalletAggregate(userId, walletItemsToTrade);
                wallet.Sell(walletTradeCommand);
            });

            _logger.LogInformation("User {UserId} successfully sold {Amount} {Symbol}",
                userId, coinTradeDto.CoinAmount, coinTradeDto.CoinSymbol);
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
