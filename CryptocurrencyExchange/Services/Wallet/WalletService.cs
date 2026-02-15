using CryptocurrencyExchange.Core.Interfaces;
using CryptocurrencyExchange.Core.Interfaces.Repositories;
using CryptocurrencyExchange.Core.Interfaces.Services;
using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Exceptions;
using CryptocurrencyExchange.Utilities;

namespace CryptocurrencyExchange.Services.Wallet
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

                Buy(tradeItems.BaseCurrency, tradeItems.TradedCurrency, usd, tradeCoinPrice);
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

                Sell(tradeItems.BaseCurrency, tradeItems.TradedCurrency, amount, tradeCoinPrice);
            });
        }

        internal void Buy(WalletItem usdt, WalletItem coin, decimal usd, decimal coinPrice)
        {
            if (usdt.Amount < usd)
                throw new InsufficientFundsException("USDT");

            var amountToBuy = usd / coinPrice;

            usdt.Amount -= usd;
            usdt.Amount = MoneyPolicyUtils.RoundFiat(usdt.Amount);

            coin.Amount += amountToBuy;
            coin.Amount = MoneyPolicyUtils.RoundCoinAmountUpTo1USD(coin.Amount, coinPrice);
        }

        internal void Sell(WalletItem usdt, WalletItem coinToSell, decimal amount, decimal coinPrice)
        {
            if (coinToSell.Amount < amount)
                throw new InsufficientFundsException(coinToSell.Symbol.ToUpper());

            var usdtAmount = coinPrice * amount;

            coinToSell.Amount -= amount;
            coinToSell.Amount = MoneyPolicyUtils.RoundCoinAmountUpTo1USD(coinToSell.Amount, coinPrice);

            usdt.Amount += usdtAmount;
            usdt.Amount = MoneyPolicyUtils.RoundFiat(usdt.Amount);
        }
    }
}
