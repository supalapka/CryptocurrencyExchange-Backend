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
            using var transaction = await _dataContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            await _dataContext.Database.ExecuteSqlRawAsync($"SELECT * FROM WalletItems WITH (TABLOCKX) WHERE UserId = {userId} AND Symbol = 'usdt'");;

            var userUsdt = await _dataContext.WalletItems
                        .Where(x => x.UserId == userId && x.Symbol == "usdt")
                        .FirstAsync();

                    if (userUsdt.Amount < (double)futureDto.Margin)
                    {
                        throw new Exception("Not enough balance in usdt to start future position");
                    }

                    var future = new Future();
                    future.Symbol = futureDto.Symbol;
                    future.EntryPrice = (double)futureDto.EntryPrice;
                    future.Margin = futureDto.Margin;
                    future.UserId = userId;
                    future.IsCompleted = false;
                    future.Leverage = futureDto.Leverage;
                    future.Position = futureDto.Position;

                    userUsdt.Amount -= (double)futureDto.Margin;

                     _dataContext.Futures.Add(future);
                     _dataContext.SaveChanges();
                     await transaction.CommitAsync();
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
                    EntryPrice = Convert.ToDecimal(position.EntryPrice),
                    Margin = position.Margin,
                    Id = position.Id,
                    Symbol = position.Symbol,
                };
                result.Add(futureDto);
            }
            result.Reverse();
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

            using var transaction = await _dataContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            await _dataContext.Database.ExecuteSqlRawAsync($"SELECT * FROM WalletItems WITH (TABLOCKX) WHERE UserId = {position.UserId} AND Symbol = 'usdt'"); ;

            var futureHistory = new FutureHistory()
            {
                FutureId = position.Id,
                Id = position.Id,
                IsLiquidated = false,
                MarkPrice = markPrice,
            };

            await _dataContext.FutureHistory.AddAsync(futureHistory);

            var userUsdt = _dataContext.WalletItems
                .Where(x => x.UserId == position.UserId && x.Symbol == "usdt")
                .First();

            userUsdt.Amount += (double)position.Margin;
            userUsdt.Amount += pnl;

            await _dataContext.SaveChangesAsync();
            await transaction.CommitAsync();

        }


        public List<FutureHIstoryOutput> GetHistory(int userId)
        {
            var positions = _dataContext.Futures.Where(x => x.UserId == userId
            && x.IsCompleted == true).ToList();
            List<FutureHIstoryOutput> result = new List<FutureHIstoryOutput>();

            foreach (var position in positions)
            {
                var outputPosition = new FutureHIstoryOutput()
                {
                    IsLiquidated = _dataContext.FutureHistory.
                    Where(x => x.Id == position.Id).Single().IsLiquidated,
                    Leverage = position.Leverage,
                    Margin = position.Margin,
                    Position = position.Position,
                    EntryPrice = position.EntryPrice,
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
