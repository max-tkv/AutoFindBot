using Newtonsoft.Json;

namespace AutoFindBot.Integration.Models;

public class GenerationResponse
{
    [JsonProperty("alias")]
    public string Alias { get; set; }
}