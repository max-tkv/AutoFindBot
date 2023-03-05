using Newtonsoft.Json;

namespace AutoFindBot.Integration.Models.TradeDealerClient;

public class GenerationResponse
{
    [JsonProperty("alias")]
    public string Alias { get; set; }
}