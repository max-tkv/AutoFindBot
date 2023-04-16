using Newtonsoft.Json;

namespace AutoFindBot.Integration.AutoRu.Models;

public class AutoRuResponseOfferStateImageUrl
{
    [JsonProperty("sizes")]
    public Dictionary<string, string> Sizes { get; set; }
}