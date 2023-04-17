using CryptocurrencyExchange.Data;
using CryptocurrencyExchange.Models;
using Microsoft.EntityFrameworkCore;

namespace CryptocurrencyExchange.Services
{
    public class FuturesService : IFuturesService
    {
        private readonly DataContext _dataContext;

        public FuturesService(DataContext context)
        {
            _dataContext = context;
        }


        public async Task CreateFutureAsync(FutureDto futureDto, int userId)
        {
            var userUsdt = await _dataContext.WalletItems.Where(x => x.Users.Id == userId
            && x.Symbol == "usdt").FirstAsync();
            if (userUsdt.Amount < (double)futureDto.Margin)
                throw new Exception("Not anough balance in usdt to start future position");

            var future = new Future();
            future.Symbol = futureDto.Symbol;
            future.Price = futureDto.Price;
            future.Margin = futureDto.Margin;
            future.UserId = userId;
            future.IsCompleted = false;
            future.Leverage = futureDto.Leverage;
            future.Position = futureDto.Position;

            userUsdt.Amount -= (double)futureDto.Margin;

            _dataContext.Futures.Add(future);
            await _dataContext.SaveChangesAsync();
        }

        public List<FutureDto> GetFuturePositions(int userId)
        {
            var positions = _dataContext.Futures.Where(x => x.UserId == userId
            && !x.IsCompleted).ToList();

            List<FutureDto> result = new List<FutureDto>();
            foreach (var position in positions)
            {
                FutureDto futureDto = new FutureDto()
                {
                    Leverage = position.Leverage,
                    Position = position.Position,
                    Price = position.Price,
                    Margin = position.Margin,
                    Id = position.Id,
                    Symbol = position.Symbol,
                };
                result.Add(futureDto);
            }

            return result;
        }
    }
}
