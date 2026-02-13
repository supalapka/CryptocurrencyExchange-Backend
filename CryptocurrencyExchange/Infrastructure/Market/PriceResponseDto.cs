using Newtonsoft.Json;

namespace CryptocurrencyExchange.Infrastructure.Market
{
    public class PriceResponseDto
    {
        [JsonProperty("price")]
        public decimal? Price { get; set; }
    }
}
