using System.Security.Cryptography;
using System.Text;

namespace CryptocurrencyExchange.Core.Models
{
    public class ApiKey
    {
        public int Id { get; private set; }
        public int UserId { get; private set; }
        public byte[] KeyHash { get; private set; }
        public string KeyPrefix { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private ApiKey() { }

        public ApiKey(int userId, byte[] keyHash, string keyPrefix, DateTime createdAt)
        {
            UserId = userId;
            KeyHash = keyHash;
            KeyPrefix = keyPrefix;
            CreatedAt = createdAt;
        }

        public static (ApiKey apiKey, string rawKey) Create(int userId)
        {
            var randomBytes = RandomNumberGenerator.GetBytes(32);
            var rawKey = Convert.ToBase64String(randomBytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');
            var keyPrefix = rawKey[..8];
            var keyHash = SHA256.HashData(Encoding.UTF8.GetBytes(rawKey));
            return (new ApiKey(userId, keyHash, keyPrefix, DateTime.UtcNow), rawKey);
        }
    }
}
