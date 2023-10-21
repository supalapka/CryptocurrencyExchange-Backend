using CryptocurrencyExchange.Data;
using CryptocurrencyExchange.Models;
using CryptocurrencyExchange.Utilities;
using Microsoft.EntityFrameworkCore;

namespace CryptocurrencyExchange.Services
{
    public class WalletService : IWalletService
    {
        private readonly DataContext _dataContext;
        private readonly INotificationService _notificationService;
        private readonly IMarketService _marketService;


        public WalletService(DataContext context, INotificationService notificatinService, IMarketService marketService)
        {
            _dataContext = context;
            _notificationService = notificatinService;
            _marketService = marketService;
        }

        public WalletService(DataContext context, IMarketService marketService)
        {
            _dataContext = context;
            _marketService = marketService;
        }


        public async Task BuyAsync(int userId, string coinSymbol, double usd)
        {
            coinSymbol = coinSymbol.ToLower();

            var USDTWalletItem = await _dataContext.WalletItems
                .FirstAsync(x => x.UserId == userId && x.Symbol == "usdt");

            if (USDTWalletItem.Amount < usd)
                throw new Exception("Not enough balance in USDT");

            decimal coinPrice = await _marketService.GetPrice(coinSymbol);
            var amountToBuy = (decimal)usd / coinPrice;
            amountToBuy = UtilFunсtions.RoundCoinAmountUpTo1USD(amountToBuy, coinPrice);

            var coinToBuy = GeWalletItem(userId, coinSymbol);
            if (coinToBuy == null)
            {
                coinToBuy = new WalletItem()
                {
                    Symbol = coinSymbol,
                    Amount = 0,
                    User = await _dataContext.Users.FirstAsync(x => x.Id == userId)
                };
                await _dataContext.WalletItems.AddAsync(coinToBuy);
            }
            USDTWalletItem.Amount -= usd;
            coinToBuy.Amount += (double)amountToBuy;

            if (USDTWalletItem.Amount < (double)0.1)
                USDTWalletItem.Amount = 0.0; //beta commission and fix the issue 0.00054993 usd on balance


            await _dataContext.SaveChangesAsync();
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


        public WalletItem GeWalletItem(int userId, string symbol)
        {
            return _dataContext.WalletItems.FirstOrDefault(x => x.UserId == userId
                && x.Symbol == symbol);

        }


        public async Task SellAsync(int userId, string coinSymbol, double amount)
        {
            var coinToSell = GeWalletItem(userId, coinSymbol);
            var usdtWalletItem = GeWalletItem(userId, "usdt");

            if (coinToSell == null || coinToSell.Amount < amount)
                throw new Exception($"Not enough balance in {coinSymbol.ToUpper()}");

            var coinPrice = await _marketService.GetPrice(coinSymbol);
            var usdtAmount = (double)coinPrice * amount;

            coinToSell.Amount -= amount;
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

            await _notificationService.CreateNotification($"You have received a transfer of " +
                $"{amount} {symbol}. Please check your wallet.", receiverId);

            await _dataContext.SaveChangesAsync();
        }


    }
}
