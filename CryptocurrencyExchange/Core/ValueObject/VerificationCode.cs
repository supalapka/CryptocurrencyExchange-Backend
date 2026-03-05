namespace CryptocurrencyExchange.Core.ValueObject
{
    public readonly record struct VerificationCode
    {
        public string Value { get; }

        public VerificationCode(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length != 6 || !value.All(char.IsDigit))
                throw new ArgumentException("Verification code must be exactly 6 digits.");

            Value = value;
        }

        public override string ToString() => Value;
    }
}
