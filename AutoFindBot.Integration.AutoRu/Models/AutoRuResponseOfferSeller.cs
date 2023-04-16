using Newtonsoft.Json;

namespace AutoFindBot.Integration.AutoRu.Models;

public class AutoRuResponseOfferSeller
{
    [JsonProperty("location")]
    public AutoRuResponseOfferSellerLocation Location { get; set; }
}