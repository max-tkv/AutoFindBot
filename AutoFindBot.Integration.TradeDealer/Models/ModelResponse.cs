using Newtonsoft.Json;

namespace AutoFindBot.Integration.Models;

public class ModelResponse
{
    [JsonProperty("alias")]
    public string Alias { get; set; }
}