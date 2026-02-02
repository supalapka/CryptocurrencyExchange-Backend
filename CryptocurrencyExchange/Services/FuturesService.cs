using CryptocurrencyExchange.Data;
using CryptocurrencyExchange.Exceptions;
using CryptocurrencyExchange.Models;
using Microsoft.EntityFrameworkCore;

namespace CryptocurrencyExchange.Services
{
    public class FuturesService : IFuturesService
    {
        private const string UsdtSymbol = "usdt";

        private readonly DataContext _dataContext;

        public FuturesService(DataContext context)
        {
            _dataContext = context;
        }


        public async Task<int> CreateFutureAsync(FutureDto futureDto, int userId)
        {
            using var transaction = await _dataContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

            if (_dataContext.Database.IsRelational() && _dataContext.Database.ProviderName != "Microsoft.EntityFrameworkCore.Sqlite")
            {
                await _dataContext.Database.ExecuteSqlRawAsync($"SELECT * FROM WalletItems WITH (TABLOCKX) WHERE UserId = {userId} AND Symbol = 'usdt'");
            }

            var userUsdt = await _dataContext.WalletItems
                                .FirstAsync(x => x.UserId == userId && x.Symbol == UsdtSymbol);

            EnsureSufficientBalance(userUsdt.Amount, futureDto.Margin);

            var future = new Future
            {
                Symbol = futureDto.Symbol,
                EntryPrice = futureDto.EntryPrice,
                Margin = futureDto.Margin,
                UserId = userId,
                IsCompleted = false,
                Leverage = futureDto.Leverage,
                Position = futureDto.Position
            };

            userUsdt.Amount = Math.Round(userUsdt.Amount - futureDto.Margin, 2);

            _dataContext.Futures.Add(future);
            await _dataContext.SaveChangesAsync(); // Используй Async версию
            await transaction.CommitAsync();

            return future.Id;
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
                    EntryPrice = position.EntryPrice,
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

            EnsureNotCompleted(position.IsCompleted);

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


        public async Task ClosePosition(int id, decimal pnl, double markPrice)
        {
            var position = _dataContext.Futures.Find(id);

            EnsureNotCompleted(position.IsCompleted);

            position.IsCompleted = true;

            using var transaction = await _dataContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

            if (_dataContext.Database.IsRelational() && _dataContext.Database.ProviderName != "Microsoft.EntityFrameworkCore.Sqlite")
            {
                await _dataContext.Database.ExecuteSqlRawAsync("SELECT * FROM WalletItems WITH (TABLOCKX) WHERE UserId = {0} AND Symbol = 'usdt'", position.UserId);
            }

            var futureHistory = new FutureHistory()
            {
                FutureId = position.Id,
                MarkPrice = markPrice,
                IsLiquidated = false
            };

            await _dataContext.FutureHistory.AddAsync(futureHistory);

            var userUsdt = await _dataContext.WalletItems
                .FirstAsync(x => x.UserId == position.UserId && x.Symbol == UsdtSymbol);

            userUsdt.Amount = CalculateBalanceAfterClose(userUsdt.Amount, position.Margin, pnl);

            await _dataContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }


        public async Task<List<FutureHIstoryOutput>> GetHistoryAsync(int userId, int page)
        {
            const int positionsPerPage = 5;

            return await _dataContext.Futures
                .Where(f => f.UserId == userId && f.IsCompleted)
                .OrderByDescending(f => f.Id)
                .Skip(positionsPerPage * (page - 1))
                .Take(positionsPerPage)
                .Select(f => new FutureHIstoryOutput
                {
                    Symbol = f.Symbol,
                    EntryPrice = f.EntryPrice,
                    Margin = f.Margin,
                    Leverage = f.Leverage,
                    Position = f.Position,
                    MarkPrice = _dataContext.FutureHistory
                        .Where(h => h.FutureId == f.Id)
                        .Select(h => h.MarkPrice)
                        .FirstOrDefault(),
                    IsLiquidated = _dataContext.FutureHistory
                        .Where(h => h.FutureId == f.Id)
                        .Select(h => h.IsLiquidated)
                        .FirstOrDefault()
                })
                .ToListAsync();
        }


        internal static void EnsureSufficientBalance(decimal balance, decimal margin)
        {
            if (balance < margin)
                throw new InsufficientFundsException();
        }

        internal static decimal CalculateBalanceAfterClose(decimal currentBalance, decimal margin, decimal pnl)
        {
            return Math.Round(currentBalance + margin + pnl, 2);
        }

        internal static void EnsureNotCompleted(bool isCompleted)
        {
            if (isCompleted)
                throw new InvalidOperationException("Position is already completed.");
        }
    }
}
