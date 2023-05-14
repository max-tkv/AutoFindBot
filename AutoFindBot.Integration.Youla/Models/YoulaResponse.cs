using Newtonsoft.Json;

namespace AutoFindBot.Integration.Youla.Models;

public class YoulaResponse
{
    [JsonProperty("data")]
    public YoulaResponseData? Data { get; set; }
}