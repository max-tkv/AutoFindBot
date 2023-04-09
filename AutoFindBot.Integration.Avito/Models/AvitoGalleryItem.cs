using Newtonsoft.Json;

namespace AutoFindBot.Integration.Avito.Models;

public class AvitoGalleryItem
{
    [JsonProperty("type")]
    public string Type { get; set; }
    
    [JsonProperty("value")]
    public object Value { get; set; }
}