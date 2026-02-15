using CryptocurrencyExchange.Core.Interfaces.Repositories;
using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Services.Futures;
using Microsoft.EntityFrameworkCore;

namespace CryptocurrencyExchange.Infrastructure.Persistence.Repositories
{
    public class EfFutureRepository : IFutureRepository
    {
        private readonly DataContext dataContext;

        public EfFutureRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task AddAsync(Future future)
        {
            await dataContext.Futures.AddAsync(future);
        }

        public async Task AddPositionToHistoryAsync(Future position, double markPrice)
        {
            var futureHistiry = new FutureHistory()
            {
                FutureId = position.Id,
                Id = position.Id, //futureHistiry.id must be position.id 
                IsLiquidated = true,
                MarkPrice = markPrice,
            };

            await dataContext.FutureHistory.AddAsync(futureHistiry);
        }

        public Task<Future?> GetByIdAsync(int id)
        {
            return dataContext.Futures.FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<List<Future>> GetCompletedFuturesAsync(int userId)
        {
            return dataContext.Futures.Where(x => x.UserId == userId && !x.IsCompleted).ToListAsync();
        }

        public async Task<List<FutureHIstoryOutput>> GetHistoryAsync(int userId, int page, int positionsPerPage)
        {
            return await dataContext.Futures
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
                    MarkPrice = dataContext.FutureHistory
                        .Where(h => h.FutureId == f.Id)
                        .Select(h => h.MarkPrice)
                        .FirstOrDefault(),
                    IsLiquidated = dataContext.FutureHistory
                        .Where(h => h.FutureId == f.Id)
                        .Select(h => h.IsLiquidated)
                        .FirstOrDefault()
                })
                .ToListAsync();
        }
    }
}
