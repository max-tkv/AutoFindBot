using Newtonsoft.Json;

namespace AutoFindBot.Integration.Youla.Models;

public class YoulaResponseDataFeedItem
{
    [JsonProperty("product")]
    public YoulaResponseDataFeedItemProduct? Product { get; set; }
}