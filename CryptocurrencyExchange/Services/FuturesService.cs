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
            future.Price = (double) futureDto.Price;
            future.Margin = futureDto.Margin;
            future.UserId = userId;
            future.IsCompleted = false;
            future.Leverage = futureDto.Leverage;
            future.Position = futureDto.Position;

            userUsdt.Amount -= (double)futureDto.Margin;

            await _dataContext.Futures.AddAsync(future);
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
                    Price = Convert.ToDecimal(position.Price),
                    Margin = position.Margin,
                    Id = position.Id,
                    Symbol = position.Symbol,
                };
                result.Add(futureDto);
            }

            return result;
        }


        public async Task LiquidatePosition(int id, double markPrice)
        {
            var position = _dataContext.Futures.Find(id);
            position.IsCompleted = true;

            var futureHistiry = new FutureHistory()
            {
                FutureId = position.Id,
                Id = position.Id, //futureHistiry.id must be position.id 
                IsLiquidated = true,
                MarkPrice = markPrice,
            };
            await _dataContext.FutureHistory.AddAsync(futureHistiry);

            await _dataContext.SaveChangesAsync();
        }


        public async Task ClosePosition(int id, double pnl, double markPrice)
         {
            var position = _dataContext.Futures.Find(id);
            position.IsCompleted = true;

            var futureHistiry = new FutureHistory()
            {
                FutureId = position.Id,
                Id = position.Id, //futureHistiry.id must be position.id 
                IsLiquidated = false,
                MarkPrice = markPrice,
            };
            await _dataContext.FutureHistory.AddAsync(futureHistiry);

            var userUsdt = _dataContext.WalletItems.Where(x => x.Users.Id == position.UserId 
            && x.Symbol == "usdt").First();
            userUsdt.Amount += (double)position.Margin;
            userUsdt.Amount += pnl;

            await _dataContext.SaveChangesAsync();
        }

        public List<FutureHIstoryOutput> GetHistory(int userId)
        {
           var positions = _dataContext.Futures.Where(x=>x.UserId== userId).ToList();
            List<FutureHIstoryOutput> result = new List<FutureHIstoryOutput>();

            foreach(var position in positions)
            {
                var outputPosition = new FutureHIstoryOutput()
                {
                    IsLiquidated = _dataContext.FutureHistory.
                    Where(x=>x.Id == position.Id).Single().IsLiquidated,
                    Leverage = position.Leverage,
                    Margin = position.Margin,
                    Position = position.Position,
                    Price = position.Price,
                    MarkPrice = _dataContext.FutureHistory.
                    Where(x => x.Id == position.Id).Single().MarkPrice,
                    Symbol = position.Symbol,
                };
                result.Add(outputPosition);
            }
            return result;
        }
    }
}
