using Newtonsoft.Json;

namespace AutoFindBot.Integration.AutoRu.Models;

public class AutoRuRequest
{
    [JsonProperty("price_from")]
    public int PriceFrom { get; set; }
    
    [JsonProperty("price_to")]
    public int PriceTo { get; set; }
    
    [JsonProperty("year_from")]
    public int YearFrom { get; set; }

    [JsonProperty("section")]
    public string Section { get; set; }

    [JsonProperty("category")]
    public string Category { get; set; }

    [JsonProperty("sort")]
    public string Sort { get; set; }

    [JsonProperty("output_type")]
    public string OutputType { get; set; }

    [JsonProperty("geo_id")]
    public List<int> GeoId { get; set; }
}