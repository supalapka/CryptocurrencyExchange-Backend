namespace CryptocurrencyExchange.Core.Domain.Wallet
{
    public static class MoneyPolicy
    {
        public static decimal RoundDownWithMax1UsdLoss(decimal amount, decimal coinPrice)
        {
            decimal maxLossInCoins = 1m / coinPrice;
            decimal step = 0.1m;

            while (true)
            {
                decimal roundedAmount = Math.Floor(amount / step) * step;
                decimal loss = amount - roundedAmount;

                if (loss <= maxLossInCoins)
                    return roundedAmount;

                step /= 10;
            }
        }

        public static decimal RoundFiat(decimal amount)
        {
            return Math.Round(amount, 2);
        }
    }
}
