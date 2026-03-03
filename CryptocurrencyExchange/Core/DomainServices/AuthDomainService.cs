using CryptocurrencyExchange.Core.Interfaces;
using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Core.ValueObject.User;
using System.Security.Cryptography;
using System.Text;

namespace CryptocurrencyExchange.Core.Domain
{
    public class AuthDomainService : IAuthDomainService
    {
        public User CreateUser(Email email, Password password)
        {
            var (hash, salt) = CreatePasswordHash(password);
            return new User(email, hash, salt);
        }

        public bool VerifyPassword(Password password, User user)
        {
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computeHash.SequenceEqual(user.PasswordHash);
        }

        private static (byte[] Hash, byte[] Salt) CreatePasswordHash(Password password)
        {
            using var hmac = new HMACSHA512();
            var salt = hmac.Key;
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return (hash, salt);
        }
    }
}
