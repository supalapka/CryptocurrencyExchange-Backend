namespace CryptocurrencyExchange.Core.ValueObject
{
    public readonly record struct Balance
    {
        public decimal Value { get; }

        public Balance(decimal value)
        {
            if (value < 0)
                throw new ArgumentException("Balance cannot be negative");
            Value = value;
        }

        public static Balance Zero => new(0);

        public static Balance operator +(Balance left, decimal amount) => new(left.Value + amount);
        public static Balance operator -(Balance left, decimal amount) => new(left.Value - amount);

        public static implicit operator decimal(Balance balance) => balance.Value;

        public override string ToString() => Value.ToString();
    }
}
