using CryptocurrencyExchange.Models;
using CryptocurrencyExchange.Services.Interfaces;
using System.Security.Cryptography;

namespace CryptocurrencyExchange.Services.Authorization
{
    public class AuthDomainService : IAuthDomainService
    {
        public User CreateUser(string email, string password)
        {
            var user = new User();
            user.Email = email;
            CreatePasswordHash(password, out byte[] PasswordHash, out byte[] PasswordSalt);
            user.PasswordHash = PasswordHash;
            user.PasswordSalt = PasswordSalt;

            return user;
        }



        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public bool VerifyPassword(string password, User user)
        {
            using (var hmac = new HMACSHA512(user.PasswordSalt))
            {
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computeHash.SequenceEqual(user.PasswordHash);
            }
        }
    }
}
