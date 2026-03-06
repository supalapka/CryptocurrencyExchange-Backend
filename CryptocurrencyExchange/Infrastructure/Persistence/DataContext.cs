using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Core.ValueObject;
using CryptocurrencyExchange.Core.ValueObject.User;
using Microsoft.EntityFrameworkCore;

namespace CryptocurrencyExchange.Infrastructure.Persistence
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<WalletItem> WalletItems { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Future> Futures { get; set; }
        public DbSet<FutureHistory> FutureHistory { get; set; }
        public DbSet<StakingCoin> StakingCoins { get; set; }
        public DbSet<Staking> Staking { get; set; }
        public DbSet<LogEntry> LogEntries { get; set; }
        public DbSet<Transfer> Transfers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureValueObjectConversions(modelBuilder);
            ConfigureRelationships(modelBuilder);
            ConfigureLogEntry(modelBuilder);
        }

        private static void ConfigureRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Future>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(f => f.UserId);

            modelBuilder.Entity<FutureHistory>()
                .HasOne<Future>()
                .WithMany()
                .HasForeignKey(fh => fh.FutureId);

            modelBuilder.Entity<Staking>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(s => s.UserId);

            modelBuilder.Entity<Transfer>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(t => t.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transfer>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(t => t.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private static void ConfigureLogEntry(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LogEntry>(b =>
            {
                b.HasKey(e => e.Id);
                b.Property(e => e.Level).IsRequired().HasMaxLength(16);
                b.Property(e => e.Category).IsRequired().HasMaxLength(512);
                b.Property(e => e.Message).IsRequired();
                b.Property(e => e.Exception);
                b.Property(e => e.TimestampUtc).IsRequired();
                b.HasIndex(e => e.TimestampUtc);
                b.HasIndex(e => e.Level);
            });
        }

        private static void ConfigureValueObjectConversions(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WalletItem>()
             .Property(x => x.Symbol)
             .HasConversion(
                 v => v.Value,
                 v => new CoinSymbol(v)
             );

            modelBuilder.Entity<WalletItem>()
               .Property(x => x.Amount)
               .HasConversion(
                   v => v.Value,
                   v => new Balance(v)
               );

            modelBuilder.Entity<User>()
                .Property(x => x.Email)
                .HasConversion(
                    v => v.Value,
                    v => new Email(v)
                );

            modelBuilder.Entity<Transfer>()
                .Property(x => x.Symbol)
                .HasConversion(
                    v => v.Value,
                    v => new CoinSymbol(v)
                );

            modelBuilder.Entity<Transfer>()
                .Property(x => x.Code)
                .HasConversion(
                    v => v.Value,
                    v => new VerificationCode(v)
                );

            modelBuilder.Entity<Transfer>()
                .Property(x => x.Status)
                .HasConversion<string>();
        }
    }
}
