using CryptocurrencyExchange.NotificationService.Entities;
using Microsoft.EntityFrameworkCore;

namespace CryptocurrencyExchange.NotificationService.Persistence
{
    public class NotificationDbContext : DbContext
    {
        public DbSet<ProcessedMessage> ProcessedMessages => Set<ProcessedMessage>();
        public DbSet<NotificationOutbox> NotificationOutboxEntries => Set<NotificationOutbox>();

        public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProcessedMessage>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasIndex(x => x.Key).IsUnique();
            });

            modelBuilder.Entity<NotificationOutbox>(e =>
            {
                e.HasKey(x => x.Id);
            });
        }
    }
}
