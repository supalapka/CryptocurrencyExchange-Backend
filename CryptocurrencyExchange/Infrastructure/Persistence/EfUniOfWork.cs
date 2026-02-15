using CryptocurrencyExchange.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CryptocurrencyExchange.Infrastructure.Persistence
{
    public class EfUniOfWork : IUnitOfWork
    {
        private readonly DataContext _context;

        public EfUniOfWork(DataContext context)
        {
            _context = context;
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task ExecuteInTransactionAsync(Func<Task> action)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(
                    isolationLevel: IsolationLevel.Serializable);

            try
            {
                await action();
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
