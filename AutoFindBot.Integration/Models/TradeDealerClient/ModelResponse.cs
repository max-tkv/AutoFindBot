using Newtonsoft.Json;

namespace AutoFindBot.Integration.Models.TradeDealerClient;

public class ModelResponse
{
    [JsonProperty("alias")]
    public string Alias { get; set; }
}