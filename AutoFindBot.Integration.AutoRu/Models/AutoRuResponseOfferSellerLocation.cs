using Newtonsoft.Json;

namespace AutoFindBot.Integration.AutoRu.Models;

public class AutoRuResponseOfferSellerLocation
{
    [JsonProperty("address")]
    public string Address { get; set; }
    
    [JsonProperty("region_info")]
    public AutoRuResponseOfferSellerLocationRegionInfo RegionInfo { get; set; }
}