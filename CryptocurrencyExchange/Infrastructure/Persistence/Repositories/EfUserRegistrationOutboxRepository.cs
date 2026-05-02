using CryptocurrencyExchange.Core.Interfaces.Repositories;
using CryptocurrencyExchange.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace CryptocurrencyExchange.Infrastructure.Persistence.Repositories
{
    public class EfUserRegistrationOutboxRepository : IUserRegistrationOutboxRepository
    {
        private readonly DataContext _context;

        public EfUserRegistrationOutboxRepository(DataContext context)
            => _context = context;

        public async Task AddAsync(UserRegistrationOutbox entry)
            => await _context.UserRegistrationOutbox.AddAsync(entry);

        public async Task<List<UserRegistrationOutbox>> GetPendingAsync()
            => await _context.UserRegistrationOutbox
                .Where(e => e.ProcessedAt == null)
                .ToListAsync();
    }
}
