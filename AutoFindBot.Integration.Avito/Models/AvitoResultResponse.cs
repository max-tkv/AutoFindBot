using Newtonsoft.Json;

namespace AutoFindBot.Integration.Avito.Models;

public class AvitoResultResponse
{
    [JsonProperty("items")]
    public List<AvitoResultResponseItem> Items { get; set; }
}