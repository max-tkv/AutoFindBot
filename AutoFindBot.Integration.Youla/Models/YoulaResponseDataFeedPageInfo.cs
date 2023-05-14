using Newtonsoft.Json;

namespace AutoFindBot.Integration.Youla.Models;

public class YoulaResponseDataFeedPageInfo
{
    [JsonProperty("hasNextPage")]
    public bool HasNextPage { get; set; }
}