using Newtonsoft.Json;

namespace CryptocurrencyExchange.Services.Market
{
    public class PriceResponseDto
    {
        [JsonProperty("price")]
        public decimal? Price { get; set; }
    }
}
