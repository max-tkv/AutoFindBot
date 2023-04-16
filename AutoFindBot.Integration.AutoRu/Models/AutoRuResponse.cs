using Newtonsoft.Json;

namespace AutoFindBot.Integration.AutoRu.Models;

public class AutoRuResponse
{
    [JsonProperty("status")]
    public string Status { get; set; }
    
    [JsonProperty("offers")]
    public List<AutoRuResponseOffer> Offers { get; set; }
}