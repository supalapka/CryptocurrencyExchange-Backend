using CryptocurrencyExchange.Data;
using CryptocurrencyExchange.Exceptions;
using CryptocurrencyExchange.Models;
using CryptocurrencyExchange.Services.Interfaces;
using CryptocurrencyExchange.Services.Wallet;
using CryptocurrencyExchange.Utilities;
using Microsoft.EntityFrameworkCore;

namespace CryptocurrencyExchange.Services
{
    public class WalletService : IWalletService
    {
        private readonly DataContext _dataContext;
        //private readonly INotificationService _notificationService;
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
            //  _dataContext = context;
            // _notificationService = notificatinService;
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
            WalletItem walletItem;
            walletItem = _dataContext.WalletItems
             .FirstOrDefault(x => x.UserId == userId && x.Symbol == symbol);

            if (walletItem == null)
                return 0;

            return walletItem.Amount;
        }


        public async Task<List<WalletItem>> GetFullWalletAsync(int userId)
        {
            return await _dataContext.WalletItems.Where(x => x.UserId == userId && x.Amount > 0).ToListAsync();
        }


        public async Task<WalletItem> GeWalletItem(int userId, string symbol)
        {
            var item = _dataContext.WalletItems.FirstOrDefault(x => x.UserId == userId
                && x.Symbol == symbol);

            if (item == null)
            {
                item = new WalletItem()
                {
                    Symbol = symbol,
                    Amount = 0,
                    User = await _dataContext.Users.FirstAsync(x => x.Id == userId)
                };
                await _dataContext.WalletItems.AddAsync(item);
            }

            return item;
        }


        public async Task SellAsync(int userId, string coinSymbol, double amount)
        {
            var coinToSell = await GeWalletItem(userId, coinSymbol);
            var usdtWalletItem = await GeWalletItem(userId, "usdt");

            if (coinToSell == null || coinToSell.Amount < amount)
                throw new Exception($"Not enough balance in {coinSymbol.ToUpper()}");

            var coinPrice = await _marketService.GetPrice(coinSymbol);
            var usdtAmount = coinPrice * amount;

            coinToSell.Amount -= amount;
            coinToSell.Amount = UtilFunсtions.RoundCoinAmountUpTo1USD(coinToSell.Amount, coinPrice);

            usdtWalletItem.Amount += usdtAmount;
            usdtWalletItem.Amount = Math.Round(usdtWalletItem.Amount, 2);

            await _dataContext.SaveChangesAsync();
        }


        public async Task SendCryptoAsync(int senderId, string symbol, double amount, int receiverId)
        {
            var receiver = await _dataContext.Users.FirstAsync(x => x.Id == receiverId);
            if (receiver == null)
                throw new Exception($"Receiver id not found");

            var walletSenderItem = await _dataContext.WalletItems.FirstAsync(x => x.UserId == senderId
            && x.Symbol == symbol);

            if (walletSenderItem == null || walletSenderItem.Amount < amount)
                throw new Exception($"Not enough balance in {symbol.ToUpper()} to send");

            var walletReceiverItem = _dataContext.WalletItems.FirstOrDefault(x => x.UserId == receiverId
            && x.Symbol == symbol);

            if (walletReceiverItem == null)
            {
                walletReceiverItem = new WalletItem()
                {
                    Amount = 0,
                    Symbol = symbol,
                    UserId = receiverId
                };
                _dataContext.WalletItems.Add(walletReceiverItem);
            }
            walletSenderItem.Amount -= amount;
            walletReceiverItem.Amount += amount;

            //await _notificationService.CreateNotification($"You have received a transfer of " +
            //    $"{amount} {symbol}. Please check your wallet.", receiverId);

            await _dataContext.SaveChangesAsync();
        }


    }
}
