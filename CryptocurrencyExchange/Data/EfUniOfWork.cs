using CryptocurrencyExchange.Services.Interfaces;

namespace CryptocurrencyExchange.Data
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
    }
}
