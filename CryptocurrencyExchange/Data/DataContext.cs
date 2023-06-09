using CryptocurrencyExchange.Models;
using Microsoft.EntityFrameworkCore;

namespace CryptocurrencyExchange.Data
{
    public class DataContext :DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<WalletItem> WalletItems { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Future> Futures { get; set; }
        public DbSet<FutureHistory> FutureHistory { get; set; }
        public DbSet<StakingCoin> StakingCoins { get; set; }
        public DbSet<Staking> Staking { get; set; }
    }
}
