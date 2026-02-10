using Newtonsoft.Json.Linq;

namespace CryptocurrencyExchange.Utilities
{
    public static class MoneyPolicyUtils
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
                    return roundedAmount;
                }
            }
        }


        public async static Task<decimal> RoundCoinAmountUpTo1USD(decimal amount, string symbol)
        {
            var baseUrl = "https://api.binance.com";
            var httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
            symbol = symbol.ToUpper();
            if (!symbol.EndsWith("USDT"))
                symbol += "USDT";
            var endpoint = $"/api/v3/ticker/price?symbol={symbol}";
            var response = await httpClient.GetAsync(endpoint);
            var content = await response.Content.ReadAsStringAsync();
            var jObject = JObject.Parse(content);

            decimal coinPrice = (decimal)jObject["price"];

            return RoundCoinAmountUpTo1USD(amount, coinPrice);
        }
    }
}
