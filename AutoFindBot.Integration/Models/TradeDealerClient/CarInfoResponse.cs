using Newtonsoft.Json;

namespace AutoFindBot.Integration.Models.TradeDealerClient;

public class CarInfoResponse
{
        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("price")]
        public int Price { get; set; }

        [JsonProperty("vin")]
        public string Vin { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("publishedAt")]
        public DateTime PublishedAt { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("company")]
        public CompanyResponse Company { get; set; }

        [JsonProperty("vin_full")]
        public string VinFull { get; set; }
        
        [JsonProperty("brand")]
        public BrandResponse Brand { get; set; }
        
        [JsonProperty("generation")]
        public GenerationResponse Generation { get; set; }
        
        [JsonProperty("model")]
        public ModelResponse Model { get; set; }
}