namespace CryptocurrencyExchange.Utilities
{
    public static class UtilFunstions
    {
        public static decimal RoundCoinAmountUpTo1USD(decimal amount, decimal coinPrice)
        {
            var oneUsdToCoinPrice = 1 / coinPrice;

            for (int i = 10; ; i *= 10) // amount to buy = 7.995590181053558. we need round this to minimum steps after . but up to 1 usd -> 7.995
            {
                var roundedAmount = Math.Floor(amount * i) / i;

                var roundAmuntUpTo1Dollar = amount - roundedAmount;
                if (oneUsdToCoinPrice >= roundAmuntUpTo1Dollar)
                {
                   return  roundedAmount;
                }
            }
        }
    }
}
