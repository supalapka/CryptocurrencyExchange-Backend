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
        public DbSet<UserRegistrationOutbox> UserRegistrationOutbox { get; set; }
        public DbSet<TransferVerificationOutbox> TransferVerificationOutbox { get; set; }
        public DbSet<TransferCompletedOutbox> TransferCompletedOutbox { get; set; }
        public DbSet<TransferIdempotentRequest> TransferIdempotentRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureValueObjectConversions(modelBuilder);
            ConfigureRelationships(modelBuilder);
            ConfigureUser(modelBuilder);
            ConfigureFuture(modelBuilder);
            ConfigureLogEntry(modelBuilder);
            ConfigureUserRegistrationOutbox(modelBuilder);
            ConfigureTransferVerificationOutbox(modelBuilder);
            ConfigureTransferCompletedOutbox(modelBuilder);
            ConfigureTransferIdempotentRequest(modelBuilder);
        }

        private static void ConfigureFuture(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Future>()
                .HasIndex(f => f.UserId)
                .IncludeProperties(f => new { f.Symbol, f.Margin });
        }

        private static void ConfigureUser(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .HasMaxLength(256);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email);
        }

        private static void ConfigureRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WalletItem>()
                .HasKey(w => new { w.UserId, w.Symbol });

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

        private static void ConfigureUserRegistrationOutbox(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRegistrationOutbox>(b =>
            {
                b.HasKey(e => e.Id);
                b.Property(e => e.Email).IsRequired().HasMaxLength(256);
                b.HasIndex(e => e.ProcessedAt);
            });
        }

        private static void ConfigureTransferVerificationOutbox(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TransferVerificationOutbox>(b =>
            {
                b.HasKey(e => e.Id);
                b.Property(e => e.Email).IsRequired().HasMaxLength(256);
                b.Property(e => e.VerificationCode).IsRequired().HasMaxLength(6);
                b.HasIndex(e => e.ProcessedAt);
            });
        }

        private static void ConfigureTransferCompletedOutbox(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TransferCompletedOutbox>(b =>
            {
                b.HasKey(e => e.Id);
                b.Property(e => e.SenderEmail).IsRequired().HasMaxLength(256);
                b.Property(e => e.ReceiverEmail).IsRequired().HasMaxLength(256);
                b.Property(e => e.Symbol).IsRequired().HasMaxLength(16);
                b.HasIndex(e => e.ProcessedAt);
            });
        }

        private static void ConfigureTransferIdempotentRequest(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TransferIdempotentRequest>(b =>
            {
                b.HasKey(r => r.Id);
                b.Property(r => r.Key).IsRequired().HasMaxLength(128);
                b.HasIndex(r => new { r.Key, r.UserId }).IsUnique();
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
