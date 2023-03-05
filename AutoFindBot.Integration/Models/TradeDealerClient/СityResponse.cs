using Newtonsoft.Json;

namespace AutoFindBot.Integration.Models.TradeDealerClient;

public class СityResponse
{
    [JsonProperty("title")]
    public string Title { get; set; }
}