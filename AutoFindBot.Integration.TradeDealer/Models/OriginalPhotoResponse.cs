using Newtonsoft.Json;

namespace AutoFindBot.Integration.Models;

public class OriginalPhotoResponse
{
    [JsonProperty("path")]
    public string Path { get; set; }
    
    [JsonProperty("width")]
    public string Width { get; set; }
    
    [JsonProperty("height")]
    public string Height { get; set; }
}