using Microsoft.EntityFrameworkCore;

namespace CryptocurrencyExchange.EmailService.Persistence
{
    public class EmailDbContext : DbContext
    {
        public DbSet<SentEmail> SentEmails { get; set; }
        public DbSet<ProcessedNotification> ProcessedNotifications => Set<ProcessedNotification>();

        public EmailDbContext(DbContextOptions<EmailDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SentEmail>(b =>
            {
                b.HasKey(e => e.Id);
                b.Property(e => e.EmailAddress).IsRequired();
                b.Property(e => e.EmailType).IsRequired();
                b.HasIndex(e => new { e.EmailAddress, e.EmailType });
            });

            modelBuilder.Entity<ProcessedNotification>(b =>
            {
                b.HasKey(e => e.Id);
                b.HasIndex(e => e.OutboxEntryId).IsUnique();
            });
        }
    }
}
