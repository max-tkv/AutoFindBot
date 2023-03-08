using Newtonsoft.Json;

namespace AutoFindBot.Integration.Models;

public class BrandResponse
{
    [JsonProperty("alias")]
    public string Alias { get; set; }
}