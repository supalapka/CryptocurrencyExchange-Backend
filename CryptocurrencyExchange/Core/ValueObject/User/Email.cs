using CryptocurrencyExchange.Exceptions;
using System.Text.RegularExpressions;

namespace CryptocurrencyExchange.Core.ValueObject.User
{
    public readonly record struct Email
    {
        private static readonly Regex EmailRegex = new(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$", // "text @ text . text".
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public string Value { get; }

        public Email(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidEmailException(value);

            var trimmedEmail = value.Trim().ToLowerInvariant();

            if (!EmailRegex.IsMatch(trimmedEmail))
                throw new InvalidEmailException(value);

            Value = trimmedEmail;
        }

        public static implicit operator string(Email email) => email.Value;
        public static explicit operator Email(string value) => new(value);

        public override string ToString() => Value;
    }
}
