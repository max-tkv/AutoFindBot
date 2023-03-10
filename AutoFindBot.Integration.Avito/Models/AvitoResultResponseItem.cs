using Newtonsoft.Json;

namespace AutoFindBot.Integration.Avito.Models;

public class AvitoResultResponseItem
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("value")]
    public AvitoResultResponseItemValue Value { get; set; }
}