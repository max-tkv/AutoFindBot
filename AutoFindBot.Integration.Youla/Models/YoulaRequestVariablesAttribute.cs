using Newtonsoft.Json;

namespace AutoFindBot.Integration.Youla.Models;

public class YoulaRequestVariablesAttribute
{
    [JsonProperty("slug")]
    public string Slug { get; set; }

    [JsonProperty("value")]
    public List<string> Value { get; set; }

    [JsonProperty("from")]
    public int? From { get; set; }

    [JsonProperty("to")]
    public long? To { get; set; }
}