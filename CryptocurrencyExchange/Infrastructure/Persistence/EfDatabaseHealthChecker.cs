using CryptocurrencyExchange.Core.Interfaces;

namespace CryptocurrencyExchange.Infrastructure.Persistence
{
    public class EfDatabaseHealthChecker : IDatabaseHealthChecker
    {
        private readonly DataContext _dataContext;

        public EfDatabaseHealthChecker(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<bool> CanConnectAsync(CancellationToken cancellationToken = default)
        {
            return await _dataContext.Database.CanConnectAsync(cancellationToken);
        }
    }
}
