namespace CryptocurrencyExchange.Core.ValueObject
{
    public readonly record struct CoinSymbol
    {
        public static readonly CoinSymbol Usdt = new("usdt");
        public static readonly CoinSymbol Btc = new("btc");
        public string Value { get; }

        public CoinSymbol(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Coin symbol is empty");

            Value = value.Trim().ToLowerInvariant();
        }

        public override string ToString() => Value;
    }
}
