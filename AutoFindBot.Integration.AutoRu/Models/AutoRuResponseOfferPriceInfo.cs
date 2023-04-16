using Newtonsoft.Json;

namespace AutoFindBot.Integration.AutoRu.Models;

public class AutoRuResponseOfferPriceInfo
{
    [JsonProperty("price")]
    public string Price { get; set; }
}