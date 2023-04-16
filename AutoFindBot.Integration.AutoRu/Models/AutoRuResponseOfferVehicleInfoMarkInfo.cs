using Newtonsoft.Json;

namespace AutoFindBot.Integration.AutoRu.Models;

public class AutoRuResponseOfferVehicleInfoMarkInfo
{
    [JsonProperty("name")]
    public string Name { get; set; }
}