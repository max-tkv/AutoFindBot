using Newtonsoft.Json;

namespace AutoFindBot.Integration.Models;

public class СityResponse
{
    [JsonProperty("title")]
    public string Title { get; set; }
}