using Newtonsoft.Json;

namespace AutoFindBot.Integration.AutoRu.Models;

public class AutoRuResponseOfferVehicleInfoTechParam
{
    [JsonProperty("human_name")]
    public string HumanName { get; set; }
}