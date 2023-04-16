using Newtonsoft.Json;

namespace AutoFindBot.Integration.AutoRu.Models;

public class AutoRuResponseOfferVehicleInfoSuperGen
{
    [JsonProperty("year_from")]
    public int YearFrom { get; set; }
}