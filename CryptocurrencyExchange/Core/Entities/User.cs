using CryptocurrencyExchange.Core.ValueObject.User;

namespace CryptocurrencyExchange.Core.Models
{
    public class User
    {
        public int Id { get; private set; }
        public Email Email { get; private set; }
        public byte[] PasswordHash { get; private set; }
        public byte[] PasswordSalt { get; private set; }

        public User(Email email, byte[] passwordHash, byte[] passwordSalt)
        {
            Email = email;
            PasswordHash = passwordHash;
            PasswordSalt = passwordSalt;
        }
    }
}
