using Newtonsoft.Json;

namespace AutoFindBot.Integration.Models.TradeDealerClient;

public class BrandResponse
{
    [JsonProperty("alias")]
    public string Alias { get; set; }
}