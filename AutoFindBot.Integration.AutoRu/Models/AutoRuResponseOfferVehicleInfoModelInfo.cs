using Newtonsoft.Json;

namespace AutoFindBot.Integration.AutoRu.Models;

public class AutoRuResponseOfferVehicleInfoModelInfo
{
    [JsonProperty("name")]
    public string Name { get; set; }
}