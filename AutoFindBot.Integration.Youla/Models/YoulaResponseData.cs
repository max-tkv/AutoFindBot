using Newtonsoft.Json;

namespace AutoFindBot.Integration.Youla.Models;

public class YoulaResponseData
{
    [JsonProperty("feed")]
    public YoulaResponseDataFeed? Feed { get; set; }
}