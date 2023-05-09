using Newtonsoft.Json;

namespace AutoFindBot.Integration.AutoRu.Models;

public class AutoRuResponseOfferVehicleInfoMarkInfo
{
    [JsonProperty("code")]
    public string Code { get; set; }
    
    [JsonProperty("name")]
    public string Name { get; set; }
}