using CryptocurrencyExchange.Data.Intefraces;
using CryptocurrencyExchange.Models;
using Microsoft.EntityFrameworkCore;

namespace CryptocurrencyExchange.Data
{
    public class EfUserRepository : IUserRepository
    {
        private readonly DataContext dataContext;

        public EfUserRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }


        public async Task<bool> UserExists(string email)
        {
            return await dataContext.Users.AnyAsync(u => u.Email == email);
        }

        public async Task AddUserAsync(User user)
        {
            await dataContext.Users.AddAsync(user);
        }
    }
}
