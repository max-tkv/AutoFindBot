using Newtonsoft.Json;

namespace AutoFindBot.Integration.Models;

public class CityResponse
{
    [JsonProperty("title")]
    public string Title { get; set; }
}