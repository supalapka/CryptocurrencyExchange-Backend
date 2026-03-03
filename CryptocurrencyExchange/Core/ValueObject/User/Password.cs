using CryptocurrencyExchange.Exceptions;

namespace CryptocurrencyExchange.Core.ValueObject.User
{
    public readonly record struct Password
    {
        public string Value { get; }

        public Password(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new InvalidPasswordException();

            Value = value;
        }

        public static implicit operator string(Password password) => password.Value;
        public static explicit operator Password(string value) => new(value);

        public override string ToString() => "***";
    }
}
