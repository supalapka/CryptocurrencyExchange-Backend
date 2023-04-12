using Azure.Core;
using CryptocurrencyExchange.Data;
using CryptocurrencyExchange.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Security.Claims;

namespace CryptocurrencyExchange.Services
{
    public class WalletService : IWalletService
    {
        private readonly DataContext _dataContext;

        public WalletService(DataContext context)
        {
            _dataContext = context;
        }

        public async Task BuyAsync(int userId, string coinSymbol, double usd)
        {
            coinSymbol = coinSymbol.ToLower();

            var USDTWalletItem = await _dataContext.WalletItems
              .Where(x => x.Users.Id == userId && x.Symbol == "usdt").FirstAsync();

            if (USDTWalletItem.Amount < usd)
                throw new Exception("Not enough balance in USDT");

            decimal coinPrice = await GetPrice(coinSymbol);
            var amountToBuy = (decimal)usd / coinPrice;

            var coinToBuy = await GeWalletItemAsync(userId, coinSymbol);
            if (coinToBuy == null)
            {
                coinToBuy = new WalletItem()
                {
                    Symbol = coinSymbol,
                    Amount = 0,
                    Users = await _dataContext.Users.Where(x => x.Id == userId).FirstAsync()
                };
                await _dataContext.WalletItems.AddAsync(coinToBuy);
            }
            USDTWalletItem.Amount -= usd;
            coinToBuy.Amount += (double)amountToBuy;

            await _dataContext.SaveChangesAsync();
        }


        public async Task<double> GetCoinAmountAsync(int userId, string symbol)
        {
            var walletItem = await _dataContext.WalletItems
                 .Where(x => x.Users.Id == userId && x.Symbol == symbol)
                 .SingleOrDefaultAsync();

            if (walletItem == null)
                return 0;

            return walletItem.Amount;
        }

        public async Task<List<WalletItem>> GetFullWalletAsync(int userId)
        {
            return await _dataContext.WalletItems.Where(x => x.Users.Id == userId && x.Amount > 0).ToListAsync();
        }

        public async Task<WalletItem> GeWalletItemAsync(int userId, string symbol)
        {
            return await _dataContext.WalletItems.Where(x => x.Users.Id == userId 
            && x.Symbol == symbol).FirstAsync();
        }

        public async Task SellAsync(int userId, string coinSymbol, double amount)
        {
            var coinToSell = await GeWalletItemAsync(userId, coinSymbol);
            var usdtWalletItem = await GeWalletItemAsync(userId, "usdt");

            if (coinToSell == null || coinToSell.Amount < amount)
                throw new Exception($"Not enough balance in {coinSymbol.ToUpper()}");

            var coinPrice = await GetPrice(coinSymbol);
            var usdtAmount = (double)coinPrice * amount;

            coinToSell.Amount -= amount;
            usdtWalletItem.Amount += usdtAmount;

            await _dataContext.SaveChangesAsync();
        }

        private async Task<decimal> GetPrice(string coinSymbol)
        {
            var baseUrl = "https://api.binance.com";
            var httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
            var symbolUSDT = coinSymbol.ToUpper() + "USDT";
            var endpoint = $"/api/v3/ticker/price?symbol={symbolUSDT}";
            var response = await httpClient.GetAsync(endpoint);
            var content = await response.Content.ReadAsStringAsync();
            var jObject = JObject.Parse(content);

            return (decimal)jObject["price"];
        }
    }
}
