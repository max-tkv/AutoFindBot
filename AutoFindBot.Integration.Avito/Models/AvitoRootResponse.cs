using Newtonsoft.Json;

namespace AutoFindBot.Integration.Avito.Models;

public class AvitoRootResponse
{
    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("result")]
    public AvitoResultResponse Result { get; set; }
}