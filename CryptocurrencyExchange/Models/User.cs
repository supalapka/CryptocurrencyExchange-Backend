using Microsoft.EntityFrameworkCore;

namespace CryptocurrencyExchange.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        public List<Notification> Notifications { get; set; }
        public List<Future> Futures { get; set; }
    }
}
