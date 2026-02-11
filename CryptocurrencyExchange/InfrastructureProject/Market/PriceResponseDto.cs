using Newtonsoft.Json;

namespace CryptocurrencyExchange.InfrastructureProject.Market
{
    public class PriceResponseDto
    {
        [JsonProperty("price")]
        public decimal? Price { get; set; }
    }
}
