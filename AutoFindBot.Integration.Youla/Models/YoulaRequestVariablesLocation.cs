using Newtonsoft.Json;

namespace AutoFindBot.Integration.Youla.Models;

public class YoulaRequestVariablesLocation
{
    [JsonProperty("latitude")]
    public object Latitude { get; set; }

    [JsonProperty("longitude")]
    public object Longitude { get; set; }

    [JsonProperty("city")]
    public string City { get; set; }

    [JsonProperty("distanceMax")]
    public object DistanceMax { get; set; }
}