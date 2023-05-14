using Newtonsoft.Json;

namespace AutoFindBot.Integration.Youla.Models;

public class YoulaResponseDataFeedItemProductImage
{
    [JsonProperty("url")]
    public string Url { get; set; }
}